using CommunityToolkit.Mvvm.Input;
using FolderBrowserEx;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Cobalt.WpfUi.Core.Converters;
using Wpf.Ui.Controls;

namespace Cobalt.WpfUi.Controls.Editors;

/// <summary>
/// Editor control for folder paths with built-in folder browser and navigation capabilities.
/// Provides buttons to open folder selection dialog and navigate to the selected folder.
/// </summary>
public class FolderEditor : EditorBase<string?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FolderEditor"/> class.
    /// Sets up folder dialog and navigation commands and their associated buttons.
    /// </summary>
    public FolderEditor()
    {
        ValueConverter ??= ValueConverterFactory.CreateDefaultStringValueConverter();

        ShowFolderDialogCommand = new RelayCommand(ShowFolderDialog);
        OpenFolderCommand = new RelayCommand(
            execute: OpenFolder,
            canExecute: () => Directory.Exists(Value));

        Buttons.Add(new Button
        {
            Command = ShowFolderDialogCommand,
            Icon = new SymbolIcon(SymbolRegular.FolderOpen24, 18),
            ToolTip = "Select a folder"
        });

        Buttons.Add(new Button
        {
            Command = OpenFolderCommand,
            Icon = new SymbolIcon(SymbolRegular.OpenFolder24, 18),
            ToolTip = "Open folder"       
        });
    }

    /// <summary>
    /// Identifies the <see cref="InitialFolder"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialFolderProperty = DependencyProperty.Register(
        name: nameof(InitialFolder),
        propertyType: typeof(string),
        ownerType: typeof(FolderEditor),
        typeMetadata: new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the initial folder path for the folder selection dialog.
    /// </summary>
    public string? InitialFolder
    {
        get => (string?)GetValue(InitialFolderProperty);
        set => SetValue(InitialFolderProperty, value);
    }

    /// <summary>
    /// Gets the command used to show the folder selection dialog.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public RelayCommand ShowFolderDialogCommand { get; }

    /// <summary>
    /// Gets the command used to open the selected folder in Windows Explorer.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public RelayCommand OpenFolderCommand { get; }

    /// <summary>
    /// Called when the Value property changes. Updates the availability of the OpenFolder command.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnValueChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnValueChanged(e);

        // Update the CanExecute state of the OpenFolder command
        // based on whether the new folder path exists
        OpenFolderCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Shows the folder selection dialog and updates the Value if a folder is selected.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void ShowFolderDialog()
    {
        // Create and configure the folder browser dialog
        FolderBrowserDialog dialog = new FolderBrowserDialog();

        // Set the initial folder if specified and valid
        if (!string.IsNullOrEmpty(InitialFolder) && Directory.Exists(InitialFolder))
            dialog.InitialFolder = InitialFolder;

        // Show the dialog and update the value if a folder was selected
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            Value = dialog.SelectedFolder;
    }

    /// <summary>
    /// Opens Windows Explorer and selects the current folder.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void OpenFolder()
    {
        // Ensure the folder exists before attempting to open it
        if (!Directory.Exists(Value)) return;

        // Launch Windows Explorer with the selected folder
        // Using /select to highlight the folder in its parent directory
        Process.Start(new ProcessStartInfo()
        {
            FileName = "explorer.exe",
            Arguments = $"/select,\"{Value}\"",
            UseShellExecute = false,
        });
    }
}