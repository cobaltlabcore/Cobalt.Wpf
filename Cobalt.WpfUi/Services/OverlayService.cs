using Cobalt.WpfUi.Controls;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Cobalt.WpfUi.Services;

/// <summary>
/// Implementation of IOverlayService that provides overlay management functionality.
/// This service manages the display and hiding of overlay content within a WPF application,
/// using a ContentControl as the host container and OverlayPresenter for content presentation.
/// </summary>
public class OverlayService : IOverlayService
{
    /// <summary>
    /// The ContentControl that serves as the host container for overlay content.
    /// </summary>
    private ContentControl? _overlayHost;

    /// <summary>
    /// Sets the host control that will contain the overlay content.
    /// </summary>
    /// <param name="overlayHost">The ContentControl that will serve as the container for overlay content.</param>
    public void SetOverlayHost(ContentControl overlayHost)
        => _overlayHost = overlayHost;

    /// <summary>
    /// Displays the specified content as an overlay within the configured host control.
    /// This method first hides any existing overlay content, then creates a new OverlayPresenter
    /// and sets the provided content within it.
    /// </summary>
    /// <param name="content">The FrameworkElement to display as overlay content.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ShowAsync(FrameworkElement content)
    {
        if (_overlayHost is null)
            throw new InvalidOperationException(
                "The overlay host has not been set. " +
                "Use SetOverlayHost() to set the host control before calling ShowAsync().");
        
        // Ensure any existing overlay is hidden before showing new content
        await HideAsync();

        // Use the UI dispatcher to ensure thread-safe UI operations
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // Create a new overlay presenter to wrap the content
            var presenter = new OverlayPresenter
            {
                Content = content
            };

            // Set the presenter as the content of the overlay host
            _overlayHost.Content = presenter;
        });
    }

    /// <summary>
    /// Hides the currently displayed overlay content by clearing the overlay host's content.
    /// This method also cleans up the OverlayPresenter's content to prevent memory leaks.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task HideAsync()
    {
        if (_overlayHost is null)
            return;
        
        // Use the UI dispatcher to ensure thread-safe UI operations
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // Clean up the overlay presenter's content if it exists
            if (_overlayHost.Content is OverlayPresenter presenter)
                presenter.Content = null;

            // Clear the overlay host's content to hide the overlay
            _overlayHost.Content = null;
        });
    }
}