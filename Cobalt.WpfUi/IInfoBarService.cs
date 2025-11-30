using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Cobalt.WpfUi;

/// <summary>
/// Service for managing InfoBar notifications in WPF applications.
/// Provides functionality to show and hide info bars with different severity levels.
/// </summary>
public interface IInfoBarService
{
    /// <summary>
    /// Sets the host control where InfoBar notifications will be displayed.
    /// </summary>
    /// <param name="host">The ContentControl that will contain the InfoBar instances.</param>
    void SetInfoBarHost(ContentControl host);

    /// <summary>
    /// Displays an InfoBar with the specified severity level and message.
    /// </summary>
    /// <param name="severity">The severity level of the notification (Info, Warning, Error, Success).</param>
    /// <param name="message">The message to display in the InfoBar.</param>
    /// <param name="isClosable">Whether the InfoBar can be closed by the user. Default is true.</param>
    /// <returns>A task that completes when the InfoBar is shown.</returns>
    Task ShowAsync(InfoBarSeverity severity, string message, bool isClosable = true);

    /// <summary>
    /// Hides the currently displayed InfoBar if any.
    /// </summary>
    /// <returns>A task that completes when the InfoBar is hidden.</returns>
    Task HideAsync();
}