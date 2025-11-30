using Enigma.Cryptography.DataEncoding;

namespace Cobalt.WpfUi.Core.Converters;

/// <summary>
/// A value converter that handles conversion between hexadecimal string representations and byte arrays.
/// This converter can parse hexadecimal strings into byte arrays and format byte arrays back into
/// hexadecimal string format.
/// </summary>
public class HexadecimalValueConverter : IValueConverter<byte[]?>
{
    /// <summary>
    /// Static instance of the hex encoding service used for all conversion operations.
    /// This is shared across all instances to avoid unnecessary object creation.
    /// </summary>
    private static readonly HexService HexService = new();

    /// <inheritdoc />
    public TryParseValueDelegate<byte[]?> TryParseValue => TryDecodeBytes;

    /// <inheritdoc />
    public FormatValueDelegate<byte[]?> FormatValue => (bytes, _) => bytes is null ? null : HexService.Encode(bytes);

    /// <summary>
    /// Attempts to decode a hexadecimal string into a byte array.
    /// </summary>
    /// <param name="value">The hexadecimal string to decode. Can be null.</param>
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
            return value is not null ? (true, HexService.Decode(value)) : (false, null);
        }
        catch
        {
            // Any exception during decoding indicates invalid hexadecimal format
            return (false, null);
        }
    }
}