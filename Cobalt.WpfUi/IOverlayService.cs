using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Cobalt.WpfUi;

/// <summary>
/// Defines the contract for managing overlay content display in WPF applications.
/// Provides methods to show and hide overlay content within a specified host control.
/// </summary>
public interface IOverlayService
{
    /// <summary>
    /// Sets the host control that will contain the overlay content.
    /// </summary>
    /// <param name="overlayHost">The ContentControl that will serve as the container for overlay content.</param>
    void SetOverlayHost(ContentControl overlayHost);

    /// <summary>
    /// Displays the specified content as an overlay within the configured host control.
    /// </summary>
    /// <param name="content">The FrameworkElement to display as overlay content.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task ShowAsync(FrameworkElement content);

    /// <summary>
    /// Hides the currently displayed overlay content.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task HideAsync();
}