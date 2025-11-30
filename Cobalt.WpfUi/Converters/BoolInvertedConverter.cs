using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System;

namespace Cobalt.WpfUi.Converters;

/// <summary>
/// A WPF value converter that inverts boolean values (true becomes false, false becomes true).
/// This converter also implements MarkupExtension, allowing it to be used directly in XAML
/// without creating a resource instance.
/// </summary>
/// <example>
/// Usage in XAML:
/// &lt;CheckBox IsEnabled="{Binding IsReadOnly, Converter={local:BoolInvertedConverter}}" /&gt;
/// </example>
public class BoolInvertedConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// Converts a boolean value to its inverted equivalent.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the binding target property (not used in this converter).</param>
    /// <param name="parameter">An optional converter parameter (not used in this converter).</param>
    /// <param name="culture">The culture to use in the converter (not used in this converter).</param>
    /// <returns>The inverted boolean value if input is boolean, otherwise null.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Check if the input value is a boolean
        if (value is bool b)
            return !b; // Return the inverted boolean value
        return null; // Return null for non-boolean values
    }

    /// <summary>
    /// Converts a boolean value back to its inverted equivalent (used for two-way binding).
    /// Since inversion is symmetric, this method performs the same operation as Convert.
    /// </summary>
    /// <param name="value">The boolean value to convert back.</param>
    /// <param name="targetType">The type of the binding source property (not used in this converter).</param>
    /// <param name="parameter">An optional converter parameter (not used in this converter).</param>
    /// <param name="culture">The culture to use in the converter (not used in this converter).</param>
    /// <returns>The inverted boolean value if input is boolean, otherwise null.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Check if the input value is a boolean
        if (value is bool b)
            return !b; // Return the inverted boolean value
        return null; // Return null for non-boolean values
    }

    /// <summary>
    /// Provides the converter instance for use in XAML markup extensions.
    /// This allows the converter to be used directly in XAML without declaring it as a resource.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The current converter instance.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}