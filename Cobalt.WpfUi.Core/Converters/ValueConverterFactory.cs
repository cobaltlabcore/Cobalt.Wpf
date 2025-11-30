namespace Cobalt.WpfUi.Core.Converters;

/// <summary>
/// Provides factory methods for creating default value converter implementations for common data types.
/// </summary>
public static class ValueConverterFactory
{
    /// <summary>
    /// Creates a default string value converter that performs no conversion (pass-through).
    /// </summary>
    /// <returns>A value converter for nullable string values.</returns>
    public static IValueConverter<string?> CreateDefaultStringValueConverter() => new ValueConverter<string?>(
        str => (true, str),
        (value, _) => value);

    /// <summary>
    /// Creates a default double value converter that uses <see cref="double.TryParse(string, out double)"/>.
    /// </summary>
    /// <returns>A value converter for nullable double values.</returns>
    public static IValueConverter<double?> CreateDefaultDoubleValueConverter() => new ValueConverter<double?>(
        str => double.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default float value converter that uses <see cref="float.TryParse(string, out float)"/>.
    /// </summary>
    /// <returns>A value converter for nullable float values.</returns>
    public static IValueConverter<float?> CreateDefaultFloatValueConverter() => new ValueConverter<float?>(
        str => float.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default decimal value converter that uses <see cref="decimal.TryParse(string, out decimal)"/>.
    /// </summary>
    /// <returns>A value converter for nullable decimal values.</returns>
    public static IValueConverter<decimal?> CreateDefaultDecimalValueConverter() => new ValueConverter<decimal?>(
        str => decimal.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default int value converter that uses <see cref="int.TryParse(string, out int)"/>.
    /// </summary>
    /// <returns>A value converter for nullable int values.</returns>
    public static IValueConverter<int?> CreateDefaultIntValueConverter() => new ValueConverter<int?>(
        str => int.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default uint value converter that uses <see cref="uint.TryParse(string, out uint)"/>.
    /// </summary>
    /// <returns>A value converter for nullable uint values.</returns>
    public static IValueConverter<uint?> CreateDefaultUIntValueConverter() => new ValueConverter<uint?>(
        str => uint.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default short value converter that uses <see cref="short.TryParse(string, out short)"/>.
    /// </summary>
    /// <returns>A value converter for nullable short values.</returns>
    public static IValueConverter<short?> CreateDefaultShortValueConverter() => new ValueConverter<short?>(
        str => short.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default ushort value converter that uses <see cref="ushort.TryParse(string, out ushort)"/>.
    /// </summary>
    /// <returns>A value converter for nullable ushort values.</returns>
    public static IValueConverter<ushort?> CreateDefaultUShortValueConverter() => new ValueConverter<ushort?>(
        str => ushort.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default long value converter that uses <see cref="long.TryParse(string, out long)"/>.
    /// </summary>
    /// <returns>A value converter for nullable long values.</returns>
    public static IValueConverter<long?> CreateDefaultLongValueConverter() => new ValueConverter<long?>(
        str => long.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default ulong value converter that uses <see cref="ulong.TryParse(string, out ulong)"/>.
    /// </summary>
    /// <returns>A value converter for nullable ulong values.</returns>
    public static IValueConverter<ulong?> CreateDefaultULongValueConverter() => new ValueConverter<ulong?>(
        str => ulong.TryParse(str, out var result) ? (true, result) : (false, null),
        (value, format) => format is null ? value?.ToString() : value?.ToString(format));

    /// <summary>
    /// Creates a default hexadecimal value converter for byte arrays.
    /// </summary>
    /// <returns>A value converter that handles hexadecimal encoding/decoding of byte arrays.</returns>
    public static IValueConverter<byte[]?> CreateDefaultHexadecimalValueConverter() => new HexadecimalValueConverter();

    /// <summary>
    /// Creates a default Base64 value converter for byte arrays.
    /// </summary>
    /// <returns>A value converter that handles Base64 encoding/decoding of byte arrays.</returns>
    public static IValueConverter<byte[]?> CreateDefaultBase64ValueConverter() => new Base64ValueConverter();
}