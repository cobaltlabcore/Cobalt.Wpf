namespace Cobalt.WpfUi.Core.Converters;

/// <summary>
/// Delegate for attempting to parse a string value into a specific type.
/// </summary>
/// <typeparam name="T">The target type to parse to.</typeparam>
/// <param name="value">The string value to parse.</param>
/// <returns>A tuple containing a boolean indicating success and the parsed value if successful, or default if failed.</returns>
public delegate (bool, T?) TryParseValueDelegate<T>(string? value);

/// <summary>
/// Delegate for formatting a value of a specific type into a string representation.
/// </summary>
/// <typeparam name="T">The type of the value to format.</typeparam>
/// <param name="value">The value to format.</param>
/// <param name="format">Optional format string to control the output format.</param>
/// <returns>The formatted string representation of the value, or null if formatting fails.</returns>
public delegate string? FormatValueDelegate<in T>(T? value, string? format);

/// <summary>
/// Interface for bidirectional value conversion between strings and strongly-typed values.
/// Provides functionality to parse strings into typed values and format typed values back to strings.
/// </summary>
/// <typeparam name="T">The type that this converter handles.</typeparam>
public interface IValueConverter<T>
{
    /// <summary>
    /// Gets the delegate used to attempt parsing string values into the target type.
    /// </summary>
    TryParseValueDelegate<T> TryParseValue { get; }

    /// <summary>
    /// Gets the delegate used to format values of the target type into string representations.
    /// </summary>
    FormatValueDelegate<T> FormatValue { get; }
}