using Cobalt.WpfUi.Core;
using Cobalt.WpfUi.Resources;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System;
using Wpf.Ui.Markup;

namespace Cobalt.WpfUi;

/// <summary>
/// A specialized bootstrapper for WPF applications that extends the base Bootstrapper
/// with WPF-specific functionality including splash screen support and WPF-UI integration.
/// </summary>
/// <remarks>
/// This class provides a complete WPF application startup solution with:
/// - Automatic WPF-UI theme and control initialization
/// - Optional splash screen with progress tracking
/// - Main window management and lifecycle
/// - WPF-specific exception handling (DispatcherUnhandledException)
/// - Factory pattern for creating windows and view models
/// </remarks>
public class WpfBootstrapper : Bootstrapper
{
    /// <summary>
    /// Gets the factory function for creating the main application window.
    /// Must be overridden in derived classes to return a valid Window instance.
    /// </summary>
    protected virtual Func<Window>? MainWindowFactory => null;
    
    /// <summary>
    /// Gets the factory function for creating the splash screen window.
    /// Required when <see cref="ShowSplashScreen"/> is true.
    /// </summary>
    protected virtual Func<Window>? SplashScreenWindowFactory => null;
    
    /// <summary>
    /// Gets the factory function for creating the splash screen view model.
    /// Must implement <see cref="IProgress"/> interface for progress tracking.
    /// Required when <see cref="ShowSplashScreen"/> is true.
    /// </summary>
    protected virtual Func<IProgress>? SplashScreenViewModelFactory => null;
    
    /// <summary>
    /// Gets the main application window instance after it's created.
    /// </summary>
    public Window? MainWindow { get; private set; }
    
    /// <summary>
    /// Gets the splash screen window instance after it's created.
    /// Only available when <see cref="ShowSplashScreen"/> is true.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected Window? SplashScreenWindow { get; private set; }
    
    /// <summary>
    /// Gets the splash screen view model instance after it's created.
    /// Only available when <see cref="ShowSplashScreen"/> is true.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected IProgress? SplashScreenViewModel { get; private set; }
    
    /// <summary>
    /// Gets a value indicating whether to show a splash screen during application startup.
    /// Override this property to enable splash screen functionality.
    /// </summary>
    protected virtual bool ShowSplashScreen => false;
    
    /// <summary>
    /// Gets the minimum duration to show the splash screen.
    /// The splash screen will remain visible for at least this duration,
    /// even if <see cref="SplashScreenAction"/> completes earlier.
    /// </summary>
    protected virtual TimeSpan SplashScreenDuration => TimeSpan.FromSeconds(2);
    
    /// <summary>
    /// Gets the action to execute while the splash screen is displayed.
    /// This can be used for initialization tasks that should occur during startup.
    /// The <see cref="IProgress"/> parameter can be used to update the splash screen progress.
    /// </summary>
    protected virtual Func<IProgress, Task>? SplashScreenAction => null;

    /// <summary>
    /// Performs the internal WPF-specific startup operations.
    /// This method is called by the base class after the host is started.
    /// </summary>
    protected override void InternalStart()
    {
        // Initialize WPF-UI themes and controls
        InitializeWpfUi();
        
        // Set the WPF-UI theme
        SetWpfUiTheme();
        
        // Create window instances and set up data contexts
        CreateSplashScreenAndWindow();
        
        // Show the appropriate window(s) based on configuration
        ShowSplashScreenAndWindow();
    }

    /// <summary>
    /// Initializes WPF-UI resource dictionaries and applies the dark theme.
    /// This method adds the necessary WPF-UI controls and theme resources to the application.
    /// </summary>
    private void InitializeWpfUi()
    {
        try
        {
            // Add WPF-UI resource dictionaries to enable modern controls
            var controlsDictionary = new ControlsDictionary();
            var themesDictionary = new ThemesDictionary();
            
            // Add Cobalt.WpfUi dictionaries
            var sbControlsDictionary = new SharpBytesControlsDictionary();
            
            // Merge the dictionaries into the application resources
            Application.Current.Resources.MergedDictionaries.Add(controlsDictionary);
            Application.Current.Resources.MergedDictionaries.Add(themesDictionary);
            Application.Current.Resources.MergedDictionaries.Add(sbControlsDictionary);
        }
        catch (Exception ex)
        {
            RaiseUnhandledException(ex);
        }
    }

    /// <summary>
    /// Sets the WPF-UI theme configuration for the application.
    /// This virtual method is called after WPF-UI resource dictionaries are initialized
    /// and is intended to be overridden in derived classes to configure the desired theme.
    /// </summary>
    /// <remarks>
    /// Override this method to customize the application theme by using WPF-UI theme services
    /// such as setting the theme type (Light/Dark), accent colors, or other theme-related properties.
    /// This method is called during the startup sequence after InitializeWpfUi() completes.
    /// </remarks>
    protected virtual void SetWpfUiTheme()
    {
    }

    /// <summary>
    /// Creates the main window and splash screen instances using the provided factory methods.
    /// Also sets up the data context for the splash screen if enabled.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required factory methods return null or are not overridden.
    /// </exception>
    private void CreateSplashScreenAndWindow()
    {
        // Create the main window - this is always required
        MainWindow = MainWindowFactory?.Invoke()
                     ?? throw new InvalidOperationException(
                         "MainWindowFactory must be overriden and return a valid Window");

        // Create splash screen components only if splash screen is enabled
        if (ShowSplashScreen)
        {
            // Create the splash screen window
            SplashScreenWindow = SplashScreenWindowFactory?.Invoke() ??
                                 throw new InvalidOperationException(
                                     "SplashScreenWindowFactory must be overriden and return a valid Window when" +
                                     " ShowSplashScreen is set to true");
            
            // Create the splash screen view model for progress tracking
            SplashScreenViewModel = SplashScreenViewModelFactory?.Invoke() ??
                                    throw new InvalidOperationException(
                                        "SplashScreenViewModelFactory must be overriden and return a valid ViewModel" +
                                        " when ShowSplashScreen is set to true");
            
            // Bind the view model to the splash screen window
            SplashScreenWindow.DataContext = SplashScreenViewModel;
        }
    }
    
    /// <summary>
    /// Shows the appropriate window(s) based on the splash screen configuration.
    /// If splash screen is enabled, shows the splash screen first; otherwise shows the main window directly.
    /// </summary>
    private void ShowSplashScreenAndWindow()
    {
        if (ShowSplashScreen)
            ShowSplashScreenWindow();
        else
            ShowMainWindow();
    }
    
    /// <summary>
    /// Shows the main application window.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when MainWindow is null.</exception>
    private void ShowMainWindow()
    {
        if (MainWindow is null)
            throw new InvalidOperationException("MainWindow is null");

        MainWindow.Show();
    }
    
    /// <summary>
    /// Shows the splash screen window and sets up the loaded event handler.
    /// The loaded event triggers the splash screen logic and eventual transition to the main window.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when SplashScreenWindow is null.</exception>
    private void ShowSplashScreenWindow()
    {
        if (SplashScreenWindow is null)
            throw new InvalidOperationException("SplashScreenWindow is null"); 
        
        SplashScreenWindow.Loaded += SplashScreenWindow_Loaded;
        SplashScreenWindow.Show();
    }
    
    /// <summary>
    /// Handles the splash screen window loaded event.
    /// Executes the splash screen action (if provided) and enforces the minimum display duration
    /// before transitioning to the main window.
    /// </summary>
    /// <param name="sender">The splash screen window that was loaded</param>
    /// <param name="e">The routed event arguments</param>
    private async void SplashScreenWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Calculate when the splash screen should close (minimum duration)
            var splashScreenEnd = DateTime.Now + SplashScreenDuration;
    
            // Execute the splash screen action if provided
            if (SplashScreenAction is not null && SplashScreenViewModel is not null)
                await SplashScreenAction(SplashScreenViewModel);
    
            // Wait until the minimum splash screen duration has elapsed
            // This ensures the splash screen is visible for at least the specified duration
            while (DateTime.Now < splashScreenEnd)
                await Task.Delay(200);
    
            // Close splash screen and show main window
            SplashScreenWindow?.Close();
            ShowMainWindow();
        }
        catch (Exception ex)
        {
            RaiseUnhandledException(ex);
        }
    }

    /// <summary>
    /// Registers WPF-specific unhandled exception handlers in addition to the base class handlers.
    /// This includes handling DispatcherUnhandledException for UI thread exceptions.
    /// </summary>
    protected override void RegisterUnhandledExceptions()
    {
        Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
        base.RegisterUnhandledExceptions();
    }

    /// <summary>
    /// Unregisters WPF-specific unhandled exception handlers.
    /// </summary>
    protected override void UnregisterUnhandledExceptions()
    {
        Application.Current.DispatcherUnhandledException -= OnDispatcherUnhandledException;
        base.UnregisterUnhandledExceptions();
    }
    
    /// <summary>
    /// Handles unhandled exceptions that occur on the WPF UI thread.
    /// Marks the exception as handled and forwards it to the UnhandledException event.
    /// </summary>
    /// <param name="sender">The dispatcher that raised the exception</param>
    /// <param name="e">The dispatcher unhandled exception event arguments</param>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        RaiseUnhandledException(e.Exception);
    }
}