using Wpf.Ui.Controls;

namespace Cobalt.WpfUi;

/// <summary>
/// Defines the contract for view models that participate in navigation lifecycle events.
/// This interface allows view models to respond to navigation events such as appearing and disappearing.
/// </summary>
public interface INavigationPageViewModel
{
    /// <summary>
    /// Called when the page/view associated with this view model has appeared and is now visible to the user.
    /// This method can be used to initialize data, start timers, or perform other setup operations.
    /// </summary>
    void OnAppeared();

    /// <summary>
    /// Called when the page/view associated with this view model is about to disappear.
    /// This method allows the view model to clean up resources or cancel the navigation if needed.
    /// </summary>
    /// <param name="args">The navigation cancel event arguments that can be used to prevent navigation by setting Cancel to true.</param>
    void OnDisappearing(NavigatingCancelEventArgs args);
}