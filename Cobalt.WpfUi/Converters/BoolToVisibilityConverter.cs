using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System;

namespace Cobalt.WpfUi.Converters;

/// <summary>
/// A WPF value converter that converts boolean values to Visibility enumeration values.
/// This converter also implements MarkupExtension, allowing it to be used directly in XAML
/// without creating a resource instance.
/// </summary>
/// <example>
/// Usage in XAML:
/// &lt;Border Visibility="{Binding IsEnabled, Converter={local:BoolToVisibilityConverter}}" /&gt;
/// &lt;TextBlock Visibility="{Binding HasError, Converter={local:BoolToVisibilityConverter IsVisibleWhenTrue=False}}" /&gt;
/// </example>
public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
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
    /// Gets or sets a value indicating whether the element should be visible when the boolean value is true.
    /// Default value is true.
    /// </summary>
    /// <remarks>
    /// When set to false, the converter behaves in an inverted manner:
    /// - true becomes the VisibilityWhenNotVisible value
    /// - false becomes Visibility.Visible
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsVisibleWhenTrue { get; set; } = true;

    /// <summary>
    /// Converts a boolean value to a Visibility enumeration value.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the binding target property (should be Visibility).</param>
    /// <param name="parameter">An optional converter parameter (not used in this converter).</param>
    /// <param name="culture">The culture to use in the converter (not used in this converter).</param>
    /// <returns>
    /// Visibility.Visible if the boolean condition matches IsVisibleWhenTrue setting,
    /// otherwise returns the value specified by VisibilityWhenNotVisible property.
    /// Returns null for non-boolean input values.
    /// </returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            // Check if the boolean value should result in visible state
            if ((IsVisibleWhenTrue && b) || (!IsVisibleWhenTrue && !b))
                return Visibility.Visible;
            return VisibilityWhenNotVisible;
        }

        return null; // Return null for non-boolean values
    }

    /// <summary>
    /// Converts a Visibility enumeration value back to a boolean value (used for two-way binding).
    /// </summary>
    /// <param name="value">The Visibility value to convert back.</param>
    /// <param name="targetType">The type of the binding source property (should be bool).</param>
    /// <param name="parameter">An optional converter parameter (not used in this converter).</param>
    /// <param name="culture">The culture to use in the converter (not used in this converter).</param>
    /// <returns>
    /// A boolean value based on the Visibility state and IsVisibleWhenTrue setting.
    /// Returns null for non-Visibility input values.
    /// </returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            // Determine if the visibility represents a "visible" state
            bool isCurrentlyVisible = visibility == Visibility.Visible;

            // Return the appropriate boolean value based on IsVisibleWhenTrue setting
            if (IsVisibleWhenTrue)
                return isCurrentlyVisible; // true if visible, false if not visible
            else
                return !isCurrentlyVisible; // false if visible, true if not visible (inverted logic)
        }

        return null; // Return null for non-Visibility values
    }

    /// <summary>
    /// Provides the converter instance for use in XAML markup extensions.
    /// This allows the converter to be used directly in XAML without declaring it as a resource.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The current converter instance.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}