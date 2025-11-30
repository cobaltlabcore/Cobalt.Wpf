using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System;

namespace Cobalt.WpfUi.Converters;

/// <summary>
/// A WPF value converter that converts null/non-null values to Visibility enumeration values.
/// This converter also implements MarkupExtension, allowing it to be used directly in XAML
/// without creating a resource instance.
/// </summary>
/// <example>
/// Usage in XAML:
/// &lt;Border Visibility="{Binding SomeObject, Converter={local:NullToVisibilityConverter}}" /&gt;
/// &lt;TextBlock Visibility="{Binding ErrorMessage, Converter={local:NullToVisibilityConverter IsVisibleWhenNotNull=False}}" /&gt;
/// </example>
public class NullToVisibilityConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// Gets or sets the visibility state to use when the element should not be visible.
    /// Default value is Visibility.Collapsed.
    /// </summary>
    /// <remarks>
    /// Use Visibility.Hidden if you want the element to take up space even when not visible,
    /// or Visibility.Collapsed if you want the element to not take up any space when not visible.
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    public Visibility VisibilityWhenNotVisible { get; set; } = Visibility.Collapsed;

    /// <summary>
    /// Gets or sets a value indicating whether the element should be visible when the value is not null.
    /// Default value is true.
    /// </summary>
    /// <remarks>
    /// When set to true (default):
    /// - Non-null values become Visibility.Visible
    /// - Null values become the VisibilityWhenNotVisible value
    /// 
    /// When set to false (inverted behavior):
    /// - Null values become Visibility.Visible
    /// - Non-null values become the VisibilityWhenNotVisible value
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsVisibleWhenNotNull { get; set; } = true;

    /// <summary>
    /// Converts an object value (null or non-null) to a Visibility enumeration value.
    /// </summary>
    /// <param name="value">The object value to check for null.</param>
    /// <param name="targetType">The type of the binding target property (should be Visibility).</param>
    /// <param name="parameter">An optional converter parameter (not used in this converter).</param>
    /// <param name="culture">The culture to use in the converter (not used in this converter).</param>
    /// <returns>
    /// Visibility.Visible if the null condition matches IsVisibleWhenNotNull setting,
    /// otherwise returns the value specified by VisibilityWhenNotVisible property.
    /// </returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Check if the value's null state should result in visible state
        if ((value is not null && IsVisibleWhenNotNull) || (value is null && !IsVisibleWhenNotNull))
            return Visibility.Visible;

        // Return the configured visibility for non-visible state
        return VisibilityWhenNotVisible;
    }

    /// <summary>
    /// This converter does not support two-way binding as it's not meaningful to convert
    /// a Visibility value back to a null/non-null object state.
    /// </summary>
    /// <param name="value">The Visibility value (ignored).</param>
    /// <param name="targetType">The type of the binding source property (ignored).</param>
    /// <param name="parameter">An optional converter parameter (ignored).</param>
    /// <param name="culture">The culture to use in the converter (ignored).</param>
    /// <returns>Always returns null as reverse conversion is not supported.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => null; // ConvertBack is not supported for this converter

    /// <summary>
    /// Provides the converter instance for use in XAML markup extensions.
    /// This allows the converter to be used directly in XAML without declaring it as a resource.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The current converter instance.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}