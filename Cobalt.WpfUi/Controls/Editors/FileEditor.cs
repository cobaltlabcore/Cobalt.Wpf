using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Cobalt.WpfUi.Core.Converters;
using Wpf.Ui.Controls;

namespace Cobalt.WpfUi.Controls.Editors;

/// <summary>
/// Editor control for file paths with built-in file browser and folder navigation capabilities.
/// Provides buttons to open file selection dialog and navigate to the containing folder.
/// </summary>
public class FileEditor : EditorBase<string?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileEditor"/> class.
    /// Sets up file dialog and folder navigation commands and their associated buttons.
    /// </summary>
    public FileEditor()
    {
        ValueConverter ??= ValueConverterFactory.CreateDefaultStringValueConverter();

        ShowFileDialogCommand = new RelayCommand(ShowFileDialog);
        OpenFolderCommand = new RelayCommand(
            execute: OpenFolder,
            canExecute: () => Directory.Exists(Path.GetDirectoryName(Value)));

        Buttons.Add(new Button
        {
            Command = ShowFileDialogCommand,
            Icon = new SymbolIcon(SymbolRegular.Document24, 18),
            ToolTip = "Select a file"
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
        ownerType: typeof(FileEditor),
        typeMetadata: new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="Filter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
        name: nameof(Filter),
        propertyType: typeof(string),
        ownerType: typeof(FileEditor),
        typeMetadata: new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the initial folder path for the file dialog.
    /// </summary>
    public string? InitialFolder
    {
        get => (string?)GetValue(InitialFolderProperty);
        set => SetValue(InitialFolderProperty, value);
    }

    /// <summary>
    /// Gets or sets the file filter string for the file dialog (e.g., "Text files (*.txt)|*.txt").
    /// </summary>
    public string? Filter
    {
        get => (string?)GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    /// <summary>
    /// Gets the command used to show the file selection dialog.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public RelayCommand ShowFileDialogCommand { get; }

    /// <summary>
    /// Gets the command used to open the folder containing the selected file.
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

        OpenFolderCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Shows the file selection dialog and updates the Value if a file is selected.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void ShowFileDialog()
    {
        OpenFileDialog dialog = new OpenFileDialog();

        if (!string.IsNullOrEmpty(InitialFolder) && Directory.Exists(InitialFolder))
            dialog.InitialDirectory = InitialFolder;

        if (!string.IsNullOrEmpty(Filter))
            dialog.Filter = Filter;

        if (dialog.ShowDialog() == true)
            Value = dialog.FileName;
    }

    /// <summary>
    /// Opens Windows Explorer and selects the current file.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void OpenFolder()
    {
        var directory = Path.GetDirectoryName(Value);
        if (!Directory.Exists(directory)) return;

        Process.Start(new ProcessStartInfo()
        {
            FileName = "explorer.exe",
            Arguments = $"/select,\"{Value}\"",
            UseShellExecute = false,
        });
    }
}