using System.Windows;
using Cobalt.WpfUi.Core.Converters;

namespace Cobalt.WpfUi.Controls.Editors;

/// <summary>
/// Specifies the encoding format for data representation.
/// </summary>
public enum DataEncoding
{
    /// <summary>
    /// Hexadecimal encoding (base 16).
    /// </summary>
    Hexadecimal,
    /// <summary>
    /// Base64 encoding.
    /// </summary>
    Base64
}

/// <summary>
/// Editor control for byte array data with support for different encoding formats.
/// Allows users to input and edit binary data as hexadecimal or Base64 text.
/// </summary>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DataEditor : EditorBase<byte[]?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataEditor"/> class.
    /// Sets the default value converter to hexadecimal format.
    /// </summary>
    public DataEditor()
    {
        ValueConverter ??= ValueConverterFactory.CreateDefaultHexadecimalValueConverter();
    }

    /// <summary>
    /// Identifies the <see cref="DataEncoding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DataEncodingProperty = DependencyProperty.Register(
        name: nameof(DataEncoding),
        propertyType: typeof(DataEncoding),
        ownerType: typeof(DataEditor),
        typeMetadata: new PropertyMetadata(DataEncoding.Hexadecimal, OnDataEncodingPropertyChanged));

    /// <summary>
    /// Gets or sets the encoding format used for data representation.
    /// </summary>
    public DataEncoding DataEncoding
    {
        get => (DataEncoding) GetValue(DataEncodingProperty);
        set => SetValue(DataEncodingProperty, value);
    }

    /// <summary>
    /// Called when the DataEncoding dependency property changes.
    /// </summary>
    /// <param name="d">The dependency object that owns the property.</param>
    /// <param name="e">The event data.</param>
    private static void OnDataEncodingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((DataEditor)d).OnDataEncodingChanged(e);

    /// <summary>
    /// Called when the DataEncoding property changes. Updates the value converter to match the new encoding.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnDataEncodingChanged(DependencyPropertyChangedEventArgs e)
    {
        // Validate the new value is a valid DataEncoding enum value
        if (e.NewValue is not DataEncoding encoding) return;

        // Switch to the appropriate parser based on the selected encoding
        ValueConverter = encoding switch
        {
            DataEncoding.Hexadecimal => ValueConverterFactory.CreateDefaultHexadecimalValueConverter(),
            DataEncoding.Base64 => ValueConverterFactory.CreateDefaultBase64ValueConverter(),
            _ => ValueConverter // Keep current parser for any unexpected values
        };
    }
}