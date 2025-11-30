using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

namespace Cobalt.WpfUi.Controls;

/// <summary>
/// Represents a control that displays a frame around a group of controls.
/// This is a custom implementation of the WPF GroupBox control.
/// </summary>
public class GroupBox : ContentControl
{
    /// <summary>
    /// Identifies the Header dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            name: nameof(Header),
            propertyType: typeof(object),
            ownerType: typeof(GroupBox),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: null));

    /// <summary>
    /// Gets or sets the header content displayed at the top of the GroupBox.
    /// </summary>
    /// <value>
    /// The content to display as the header. This can be any object, including strings, controls, or other UI elements.
    /// The default value is null.
    /// </value>
    [Bindable(true)]
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Identifies the HeaderTemplate dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(
            name: nameof(HeaderTemplate),
            propertyType: typeof(DataTemplate),
            ownerType: typeof(GroupBox),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: null));

    /// <summary>
    /// Gets or sets the data template used to display the header content.
    /// </summary>
    /// <value>
    /// A DataTemplate that defines how the header content should be rendered.
    /// If null, the default presentation of the Header property will be used.
    /// </value>
    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?) GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
}