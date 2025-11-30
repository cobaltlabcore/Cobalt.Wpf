namespace Cobalt.WpfUi.Core.Converters;

/// <summary>
/// A generic value converter that provides parsing and formatting functionality for values of type T.
/// This class serves as a concrete implementation of <see cref="IValueConverter{T}"/> that delegates
/// the actual conversion logic to provided delegate functions.
/// </summary>
/// <typeparam name="T">The type of value this converter handles</typeparam>
public class ValueConverter<T> : IValueConverter<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueConverter{T}"/> class with the specified
    /// parsing and formatting delegates.
    /// </summary>
    /// <param name="tryParseValue">A delegate that attempts to parse a string value into type T</param>
    /// <param name="formatValue">A delegate that formats a value of type T into a string representation</param>
    public ValueConverter(TryParseValueDelegate<T> tryParseValue, FormatValueDelegate<T> formatValue)
    {
        TryParseValue = tryParseValue;
        FormatValue = formatValue;
    }

    /// <inheritdoc />
    /// <summary>
    /// Gets the delegate used to attempt parsing string values into type T.
    /// </summary>
    public TryParseValueDelegate<T> TryParseValue { get; }

    /// <inheritdoc />
    /// <summary>
    /// Gets the delegate used to format values of type T into their string representation.
    /// </summary>
    public FormatValueDelegate<T> FormatValue { get; }
}