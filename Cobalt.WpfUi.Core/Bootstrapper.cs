using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;

namespace Cobalt.WpfUi.Core;

/// <summary>
/// Represents the current operational state of a bootstrapper during its lifecycle.
/// </summary>
/// <remarks>
/// This enum defines the possible states that a bootstrapper can be in during its execution:
/// <list type="bullet">
/// <item><description><see cref="NotStarted"/> - Initial state before any operations begin</description></item>
/// <item><description><see cref="Starting"/> - Transitional state during initialization</description></item>
/// <item><description><see cref="Started"/> - Active operational state</description></item>
/// <item><description><see cref="Stopping"/> - Transitional state during shutdown</description></item>
/// <item><description><see cref="Stopped"/> - Final state after shutdown completion</description></item>
/// </list>
/// State transitions follow a defined pattern: NotStarted → Starting → Started → Stopping → Stopped
/// </remarks>

public enum BootstrapperState
{
    /// <summary>
    /// The bootstrapper has not been started yet and is in its initial state.
    /// This is the default state when a bootstrapper instance is first created.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The bootstrapper is currently in the process of starting up.
    /// This is a transitional state during initialization, host creation, and service configuration.
    /// </summary>
    Starting,

    /// <summary>
    /// The bootstrapper has successfully started and is now running.
    /// The application host is active and all services are available for use.
    /// </summary>
    Started,

    /// <summary>
    /// The bootstrapper is currently in the process of shutting down.
    /// This is a transitional state during graceful shutdown and resource cleanup.
    /// </summary>
    Stopping,

    /// <summary>
    /// The bootstrapper has completed its shutdown process and has stopped.
    /// All resources have been disposed and the host is no longer active.
    /// </summary>
    Stopped
}


/// <summary>
/// Abstract base class that provides a standardized bootstrapping mechanism for .NET applications.
/// This class manages the complete application lifecycle including host creation, service configuration, 
/// exception handling, and graceful startup/shutdown processes using the .NET Generic Host pattern.
/// </summary>
/// <remarks>
/// <para>
/// The Bootstrapper class simplifies application initialization by providing:
/// </para>
/// <list type="bullet">
/// <item><description>Automatic host creation and lifecycle management</description></item>
/// <item><description>Event-driven lifecycle notifications (Starting, Started, Stopping, Stopped)</description></item>
/// <item><description>Built-in unhandled exception handling and forwarding</description></item>
/// <item><description>Extensible configuration and service registration points</description></item>
/// </list>
/// <para>
/// To use this class, inherit from it and override the virtual methods as needed:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="ConfigureAppConfiguration"/> - Add custom configuration sources</description></item>
/// <item><description><see cref="ConfigureServices"/> - Register application services</description></item>
/// <item><description><see cref="CaptureUnhandledExceptions"/> - Control exception handling behavior</description></item>
/// </list>
/// <para>
/// The bootstrapper automatically handles both AppDomain and TaskScheduler unhandled exceptions,
/// forwarding them through the <see cref="UnhandledException"/> event for centralized error handling.
/// </para>
/// </remarks>
public abstract class Bootstrapper : IDisposable
{
    /// <summary>
    /// The .NET Generic Host instance that manages the application's services and lifecycle.
    /// This field is null when the bootstrapper is not started and contains the active host
    /// when the bootstrapper is in Started state.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected IHost? Host;
    
    /// <summary>
    /// Indicates whether the bootstrapper has been disposed.
    /// Used to prevent operations on disposed instances and ensure proper resource cleanup.
    /// </summary>
    protected bool Disposed;

    /// <summary>
    /// Gets the current state of the bootstrapper
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public BootstrapperState State { get; private set; } = BootstrapperState.NotStarted;
    
    /// <summary>
    /// Gets a value indicating whether unhandled exceptions should be captured and handled.
    /// Override this property to disable exception handling if needed.
    /// </summary>
    /// <value>True if unhandled exceptions should be captured; otherwise, false. Default is true.</value>
    protected virtual bool CaptureUnhandledExceptions => true;

    /// <summary>
    /// Starts the application host asynchronously
    /// </summary>
    /// <returns>A task that represents the asynchronous start operation</returns>
    public virtual async Task StartAsync()
    {
        ThrowIfDisposed();
        
        if (State != BootstrapperState.NotStarted)
            throw new InvalidOperationException($"Cannot start bootstrapper in state: {State}");
        
        try
        {
            State = BootstrapperState.Starting;
            RaiseStarting();

            if (CaptureUnhandledExceptions)
                RegisterUnhandledExceptions();

            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureServices(ConfigureServices);

            Host = hostBuilder.Build();

            await Host.StartAsync().ConfigureAwait(false);
            
            InternalStart();

            State = BootstrapperState.Started;
            RaiseStarted();
        }
        catch (Exception ex)
        {
            State = BootstrapperState.NotStarted;
            RaiseUnhandledException(ex);
            throw;
        }
    }
    
    /// <summary>
    /// Stops the application host asynchronously and disposes resources
    /// </summary>
    /// <returns>A task that represents the asynchronous stop operation</returns>
    public virtual async Task StopAsync()
    {
        ThrowIfDisposed();
        
        try
        {
            State = BootstrapperState.Stopping;
            RaiseStopping();

            if (Host is not null)
                await Host.StopAsync().ConfigureAwait(false);

            State = BootstrapperState.Stopped;
            RaiseStopped();
        }
        catch (Exception ex)
        {
            RaiseUnhandledException(ex);
            throw;
        }
        finally
        {
            Dispose();
        }
    }

    /// <summary>
    /// Configures the application configuration. Override this method to add custom configuration sources
    /// </summary>
    /// <param name="context">The host builder context</param>
    /// <param name="builder">The configuration builder</param>
    protected virtual void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
    {
    }

    /// <summary>
    /// Configures the services for dependency injection. Override this method to register custom services
    /// </summary>
    /// <param name="context">The host builder context</param>
    /// <param name="services">The service collection</param>
    protected virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
    }

    /// <summary>
    /// Performs internal startup operations after the host has been successfully started.
    /// This method is called during the startup process after the .NET Generic Host is started
    /// but before the bootstrapper state transitions to <see cref="BootstrapperState.Started"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This virtual method provides an extension point for derived classes to perform custom
    /// initialization logic that should occur after the host infrastructure is ready but before
    /// the application is considered fully started.
    /// </para>
    /// <para>
    /// The method is called during the <see cref="StartAsync"/> process in the following sequence:
    /// </para>
    /// <list type="number">
    /// <item><description>State changes to <see cref="BootstrapperState.Starting"/></description></item>
    /// <item><description><see cref="Starting"/> event is raised</description></item>
    /// <item><description>Host builder is created and configured</description></item>
    /// <item><description>Host is built and started</description></item>
    /// <item><description><strong>InternalStart is called</strong></description></item>
    /// <item><description>State changes to <see cref="BootstrapperState.Started"/></description></item>
    /// <item><description><see cref="Started"/> event is raised</description></item>
    /// </list>
    /// <para>
    /// Override this method in derived classes to implement application-specific startup logic
    /// such as initializing UI components, starting background services, or performing other
    /// post-host-startup operations.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Any exceptions thrown by this method will be caught by <see cref="StartAsync"/>,
    /// cause the bootstrapper state to revert to <see cref="BootstrapperState.NotStarted"/>,
    /// and be forwarded through the <see cref="UnhandledException"/> event before being re-thrown.
    /// </exception>
    protected virtual void InternalStart()
    {
    }

    /// <summary>
    /// Event raised when the bootstrapper is starting
    /// </summary>
    public event EventHandler? Starting;
    
    /// <summary>
    /// Raises the <see cref="Starting"/> event
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void RaiseStarting() => Starting?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Event raised when the bootstrapper has started successfully
    /// </summary>
    public event EventHandler? Started;
    
    /// <summary>
    /// Raises the <see cref="Started"/> event
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void RaiseStarted() => Started?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Event raised when the bootstrapper is stopping
    /// </summary>
    public event EventHandler? Stopping;
    
    /// <summary>
    /// Raises the <see cref="Stopping"/> event
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void RaiseStopping() => Stopping?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Event raised when the bootstrapper has stopped
    /// </summary>
    public event EventHandler? Stopped;
    
    /// <summary>
    /// Raises the <see cref="Stopped"/> event
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void RaiseStopped() => Stopped?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Event raised when an unhandled exception occurs
    /// </summary>
    public event EventHandler<Exception>? UnhandledException;
    
    /// <summary>
    /// Raises the <see cref="UnhandledException"/> event
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void RaiseUnhandledException(Exception ex) => UnhandledException?.Invoke(this, ex);
    
    /// <summary>
    /// Registers event handlers for unhandled exceptions from both AppDomain and TaskScheduler.
    /// Override this method to handle additional types of unhandled exceptions.
    /// </summary>
    protected virtual void RegisterUnhandledExceptions()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    /// <summary>
    /// Unregisters event handlers for unhandled exceptions from both AppDomain and TaskScheduler.
    /// Override this method to unregister additional exception handlers if you override RegisterUnhandledExceptions.
    /// </summary>
    protected virtual void UnregisterUnhandledExceptions()
    {
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
    }

    /// <summary>
    /// Handles unhandled exceptions from the current AppDomain and forwards them to the UnhandledException event.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The unhandled exception event arguments</param>
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            RaiseUnhandledException(ex);
    }

    /// <summary>
    /// Handles unobserved task exceptions from the TaskScheduler and forwards them to the UnhandledException event.
    /// Marks the exception as observed to prevent application termination.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The unobserved task exception event arguments</param>
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        RaiseUnhandledException(e.Exception);
    }
    
    private void ThrowIfDisposed()
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(Bootstrapper));
    }

    /// <inheritdoc />>
    public void Dispose()
    {
        Dispose(true);
        Disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases managed and unmanaged resources used by the bootstrapper.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged resources; 
    /// <see langword="false"/> to release only unmanaged resources.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method implements the standard dispose pattern. When <paramref name="disposing"/> is true,
    /// it performs cleanup of managed resources including:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Unregistering unhandled exception handlers if they were registered</description></item>
    /// <item><description>Disposing the application host instance</description></item>
    /// <item><description>Clearing the host reference</description></item>
    /// <item><description>Setting the disposed flag to prevent further operations</description></item>
    /// </list>
    /// <para>
    /// This method is called by both <see cref="Dispose()"/> and the finalizer (if implemented).
    /// It ensures that resources are properly released and prevents multiple disposal attempts.
    /// </para>
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed && disposing)
        {
            if (CaptureUnhandledExceptions)
                UnregisterUnhandledExceptions();
            
            Host?.Dispose();
            Host = null;
        }
    }
}