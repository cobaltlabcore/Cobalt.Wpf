namespace Cobalt.WpfUi;

/// <summary>
/// Defines the contract for navigation pages in the WPF UI framework.
/// This interface provides a common structure for pages that participate in navigation
/// and require access to their associated view models.
/// </summary>
public interface INavigationPage
{
    /// <summary>
    /// Gets the view model associated with this navigation page.
    /// This property provides access to the page's data context and business logic.
    /// </summary>
    /// <value>The view model implementing <see cref="INavigationPageViewModel"/>.</value>
    INavigationPageViewModel ViewModel { get; }
}

/// <summary>
/// Defines a strongly-typed contract for navigation pages with a specific view model type.
/// This generic interface extends <see cref="INavigationPage"/> to provide compile-time
/// type safety when working with specific view model implementations.
/// </summary>
/// <typeparam name="T">The specific type of view model that must implement <see cref="INavigationPageViewModel"/>.</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface INavigationPage<T> : INavigationPage where T : INavigationPageViewModel
{
    /// <summary>
    /// Gets the strongly-typed view model associated with this navigation page.
    /// This property provides type-safe access to the specific view model implementation,
    /// eliminating the need for casting when accessing view model-specific members.
    /// </summary>
    /// <value>The view model of type <typeparamref name="T"/>.</value>
    /// <remarks>
    /// This property shadows the base <see cref="INavigationPage.ViewModel"/> property
    /// to provide the specific type, while still maintaining compatibility with the base interface.
    /// </remarks>
    new T ViewModel { get; }
}