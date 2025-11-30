using Button = System.Windows.Controls.Button;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using TextBox = System.Windows.Controls.TextBox;

namespace Cobalt.WpfUi.Controls.Editors;

/// <summary>
/// Base class for all editor controls in the SharpBytes WPF UI library.
/// Provides common functionality for custom text-based input controls with additional UI elements.
/// </summary>
public abstract class EditorBase : TextBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditorBase"/> class.
    /// Sets up default commands and initializes the buttons collection.
    /// </summary>
    protected EditorBase()
    {
        Buttons = [];

        ClearCommand = new RelayCommand(ClearEditor);
        CopyCommand = new RelayCommand(() => Clipboard.SetText(Text));
    }

    /// <summary>
    /// Identifies the <see cref="TextBoxHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextBoxHeightProperty = DependencyProperty.Register(
        name: nameof(TextBoxHeight),
        propertyType: typeof(double),
        ownerType: typeof(EditorBase),
        typeMetadata: new PropertyMetadata(double.NaN));

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        name: nameof(Title),
        propertyType: typeof(string),
        ownerType: typeof(EditorBase),
        typeMetadata: new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="SelectAllTextOnFocus"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectAllTextOnFocusProperty = DependencyProperty.Register(
        name: nameof(SelectAllTextOnFocus),
        propertyType: typeof(bool),
        ownerType: typeof(EditorBase),
        typeMetadata: new PropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="Buttons"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(
        name: nameof(Buttons),
        propertyType: typeof(List<Wpf.Ui.Controls.Button>),
        ownerType: typeof(EditorBase),
        typeMetadata: new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="Suffix"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(
        name: nameof(Suffix),
        propertyType: typeof(string),
        ownerType: typeof(EditorBase),
        typeMetadata: new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the height of the text box portion of the editor.
    /// </summary>
    public double TextBoxHeight
    {
        get => (double)GetValue(TextBoxHeightProperty);
        set => SetValue(TextBoxHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the title displayed above the editor control.
    /// </summary>
    public string? Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether all text should be selected when the control receives focus.
    /// </summary>
    public bool SelectAllTextOnFocus
    {
        get => (bool)GetValue(SelectAllTextOnFocusProperty);
        set => SetValue(SelectAllTextOnFocusProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of buttons displayed alongside the editor.
    /// </summary>
    public List<Wpf.Ui.Controls.Button> Buttons
    {
        get => (List<Wpf.Ui.Controls.Button>)GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    /// <summary>
    /// Gets or sets the suffix text displayed after the editor content.
    /// </summary>
    public string? Suffix
    {
        get => (string?)GetValue(SuffixProperty);
        set => SetValue(SuffixProperty, value);
    }

    /// <summary>
    /// Gets the command used to clear the editor content.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public ICommand ClearCommand { get; protected set; }

    /// <summary>
    /// Gets the command used to copy the editor content to the clipboard.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public ICommand CopyCommand { get; protected set; }

    /// <summary>
    /// When implemented in a derived class, clears the editor content.
    /// </summary>
    protected abstract void ClearEditor();

    /// <summary>
    /// Called when the editor loses keyboard focus. Scrolls to the home position if SelectAllTextOnFocus is enabled.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        if (SelectAllTextOnFocus)
            ScrollToHome();
    }

    // Legacy implementation - kept as reference
    // protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    // {
    //     base.OnPreviewMouseLeftButtonDown(e);
    //     if (!IsKeyboardFocusWithin)
    //     {
    //         Focus();
    //         e.Handled = true;
    //     }
    // }

    /// <summary>
    /// Called when a mouse button is pressed down over the editor. 
    /// Handles focus behavior while preventing interference with editor buttons.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);

        if (!IsKeyboardFocusWithin)
        {
            var originalSource = e.OriginalSource as DependencyObject;

            if (IsMouseOverEditorButton(originalSource))
            {
                e.Handled = false;
                return;
            }

            Focus();
            e.Handled = true;
        }
    }

    /// <summary>
    /// Determines whether the mouse is currently over an editor button by traversing the visual tree.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <returns>True if the element or any of its ancestors is a Button; otherwise, false.</returns>
    private bool IsMouseOverEditorButton(DependencyObject? element)
    {
        while (element != null)
        {
            if (element is Button)
                return true;

            element = VisualTreeHelper.GetParent(element);
        }
        return false;
    }

    /// <summary>
    /// Called when the editor receives keyboard focus. Selects all text if SelectAllTextOnFocus is enabled.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);

        if (SelectAllTextOnFocus)
            SelectAll();
    }
}