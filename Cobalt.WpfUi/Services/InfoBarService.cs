using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Cobalt.WpfUi.Services;

/// <summary>
/// Implementation of IInfoBarService that manages InfoBar notifications in WPF applications.
/// This service handles displaying and hiding info bars with proper UI thread marshalling.
/// </summary>
public class InfoBarService : IInfoBarService
{
    /// <summary>
    /// The host control where InfoBar instances will be displayed.
    /// </summary>
    private ContentControl? _infoBarHost;

    /// <summary>
    /// Sets the host control where InfoBar notifications will be displayed.
    /// </summary>
    /// <param name="host">The ContentControl that will contain the InfoBar instances.</param>
    public void SetInfoBarHost(ContentControl host)
        => _infoBarHost = host;

    /// <summary>
    /// Displays an InfoBar with the specified severity level and message.
    /// This method ensures any existing InfoBar is hidden before showing the new one.
    /// </summary>
    /// <param name="severity">The severity level of the notification.</param>
    /// <param name="message">The message to display in the InfoBar.</param>
    /// <param name="isClosable">Whether the InfoBar can be closed by the user.</param>
    /// <returns>A task that completes when the InfoBar is shown.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the InfoBar host has not been set.</exception>
    public async Task ShowAsync(InfoBarSeverity severity, string message, bool isClosable = true)
    {
        // Ensure the host has been configured before attempting to show notifications
        if (_infoBarHost is null)
            throw new InvalidOperationException("The info bar host has not been set. " +
                                                "Use SetInfoBarHost() to set the host control before calling ShowInfoBarAsync().");

        // Hide any existing InfoBar before showing the new one to prevent overlapping
        await HideAsync();

        // Ensure UI operations are performed on the UI thread
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // Create and configure the new InfoBar
            var infoBar = new InfoBar
            {
                Severity = severity,
                Message = message,
                IsOpen = true,
                IsClosable = isClosable
            };

            // Display the InfoBar in the host control
            _infoBarHost.Content = infoBar;
        });
    }

    /// <summary>
    /// Hides the currently displayed InfoBar if any.
    /// This method is safe to call even when no InfoBar is currently displayed.
    /// </summary>
    /// <returns>A task that completes when the InfoBar is hidden.</returns>
    public async Task HideAsync()
    {
        // Nothing to hide if no host is set
        if (_infoBarHost is null)
            return;

        // Ensure UI operations are performed on the UI thread
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // Clear the host content to hide the InfoBar
            _infoBarHost.Content = null;
        });
    }
}