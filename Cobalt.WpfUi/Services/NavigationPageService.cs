using Wpf.Ui.Controls;

namespace Cobalt.WpfUi.Services;

/// <summary>
/// Service that manages navigation events and page lifecycle for WPF UI navigation.
/// Handles view model lifecycle events when navigating between pages.
/// </summary>
public class NavigationPageService
{
    /// <summary>
    /// Tracks the currently active navigation page to manage its view model lifecycle.
    /// </summary>
    private INavigationPage? _currentPage;

    /// <summary>
    /// Registers navigation event handlers to the specified NavigationView.
    /// This enables automatic view model lifecycle management during navigation.
    /// </summary>
    /// <param name="navigationView">The NavigationView to register events for</param>
    public void RegisterNavigationEvents(NavigationView navigationView)
    {
        navigationView.Navigating += OnNavigating;
        navigationView.Navigated += OnNavigated;
    }

    /// <summary>
    /// Unregisters navigation event handlers from the specified NavigationView.
    /// Should be called when disposing or when navigation management is no longer needed.
    /// </summary>
    /// <param name="navigationView">The NavigationView to unregister events from</param>
    public void UnregisterNavigationEvents(NavigationView navigationView)
    {
        navigationView.Navigating -= OnNavigating;
        navigationView.Navigated -= OnNavigated;
    }

    /// <summary>
    /// Handles the Navigating event to notify the current page's view model that it's about to disappear.
    /// This allows the view model to perform cleanup or validation before navigation completes.
    /// </summary>
    /// <param name="sender">The NavigationView that triggered the event</param>
    /// <param name="args">Event arguments containing navigation details and cancellation capability</param>
    private void OnNavigating(object sender, NavigatingCancelEventArgs args)
        => _currentPage?.ViewModel.OnDisappearing(args);

    /// <summary>
    /// Handles the Navigated event to update the current page reference and notify the new page's view model.
    /// This occurs after navigation has completed successfully.
    /// </summary>
    /// <param name="sender">The NavigationView that triggered the event</param>
    /// <param name="args">Event arguments containing the newly navigated page</param>
    private void OnNavigated(object sender, NavigatedEventArgs args)
    {
        // Update the current page reference to the newly navigated page
        _currentPage = args.Page as INavigationPage;

        // Notify the new page's view model that it has appeared
        _currentPage?.ViewModel.OnAppeared();
    }
}