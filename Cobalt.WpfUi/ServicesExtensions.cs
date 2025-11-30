using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.DependencyInjection;
using Wpf.Ui;

namespace Cobalt.WpfUi;

/// <summary>
/// Provides extension methods for configuring dependency injection services
/// specifically for WPF UI components and navigation functionality.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Configures the dependency injection container with WPF UI navigation services.
    /// This method registers the necessary services for page navigation functionality.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddWpfUiNavigation(this IServiceCollection services)
    {
        // Register the navigation view page provider for handling page navigation
        _ = services.AddNavigationViewPageProvider();

        // Register the navigation service as a singleton for application-wide navigation management
        _ = services.AddSingleton<INavigationService, NavigationService>();
    }
    
    // /// <summary>
    // /// Registers a view and its corresponding view model as transient services in the dependency injection container.
    // /// This is a convenience method for MVVM pattern implementation where views and view models are paired together.
    // /// </summary>
    // /// <typeparam name="TView">The type of the view, which must inherit from FrameworkElement.</typeparam>
    // /// <typeparam name="TViewModel">The type of the view model, which must be a class.</typeparam>
    // /// <param name="services">The service collection to add services to.</param>
    // public static void AddViewAndViewModel<TView, TViewModel>(this IServiceCollection services)
    //     where TView : FrameworkElement
    //     where TViewModel : class
    // {
    //     // Register the view as transient - a new instance will be created each time it's requested
    //     _ = services.AddTransient<TView>();
    //
    //     // Register the view model as transient - a new instance will be created each time it's requested
    //     _ = services.AddTransient<TViewModel>();
    // }
}