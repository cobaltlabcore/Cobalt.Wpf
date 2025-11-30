using Enigma.Cryptography.DataEncoding;

namespace Cobalt.WpfUi.Core.Converters;

/// <summary>
/// A value converter that handles conversion between Base64 string representations and byte arrays.
/// This converter can parse Base64-encoded strings into byte arrays and format byte arrays back into
/// Base64 string format.
/// </summary>
public class Base64ValueConverter : IValueConverter<byte[]?>
{
    /// <summary>
    /// Static instance of the Base64 encoding service used for all conversion operations.
    /// This is shared across all instances to avoid unnecessary object creation.
    /// </summary>
    private static readonly Base64Service Base64Service = new();

    /// <inheritdoc />
    public TryParseValueDelegate<byte[]?> TryParseValue => TryDecodeBytes;

    /// <inheritdoc />
    public FormatValueDelegate<byte[]?> FormatValue => (bytes, _) => bytes is null ? null : Base64Service.Encode(bytes);

    /// <summary>
    /// Attempts to decode a Base64 string into a byte array.
    /// </summary>
    /// <param name="value">The Base64 string to decode. Can be null.</param>
    /// <returns>
    /// A tuple containing:
    /// - A boolean indicating whether the conversion was successful
    /// - The resulting byte array, or null if conversion failed or input was null
    /// </returns>
    private (bool, byte[]?) TryDecodeBytes(string? value)
    {
        try
        {
            // Return success with decoded bytes if value is not null, otherwise fail
            return value is not null ? (true, Base64Service.Decode(value)) : (false, null);
        }
        catch
        {
            // Any exception during decoding indicates invalid Base64 format
            return (false, null);
        }
    }
}