using Cobalt.WpfUi.Core.Converters;
// ReSharper disable ClassNeverInstantiated.Global

namespace Cobalt.WpfUi.Controls.Editors;

/// <summary>
/// Editor control for nullable double-precision floating-point values.
/// </summary>
public class DoubleEditor : EditorBase<double?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleEditor"/> class.
    /// </summary>
    public DoubleEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultDoubleValueConverter();
}

/// <summary>
/// Editor control for nullable single-precision floating-point values.
/// </summary>
public class FloatEditor : EditorBase<float?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FloatEditor"/> class.
    /// </summary>
    public FloatEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultFloatValueConverter();
}

/// <summary>
/// Editor control for nullable decimal values with high precision.
/// </summary>
public class DecimalEditor : EditorBase<decimal?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalEditor"/> class.
    /// </summary>
    public DecimalEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultDecimalValueConverter();
}

/// <summary>
/// Editor control for nullable 32-bit signed integer values.
/// </summary>
public class IntEditor : EditorBase<int?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntEditor"/> class.
    /// </summary>
    public IntEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultIntValueConverter();
}

/// <summary>
/// Editor control for nullable 32-bit unsigned integer values.
/// </summary>
public class UIntEditor : EditorBase<uint?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIntEditor"/> class.
    /// </summary>
    public UIntEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultUIntValueConverter();
}

/// <summary>
/// Editor control for nullable 16-bit signed integer values.
/// </summary>
public class ShortEditor : EditorBase<short?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShortEditor"/> class.
    /// </summary>
    public ShortEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultShortValueConverter();
}

/// <summary>
/// Editor control for nullable 16-bit unsigned integer values.
/// </summary>
public class UShortEditor : EditorBase<ushort?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UShortEditor"/> class.
    /// </summary>
    public UShortEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultUShortValueConverter();
}

/// <summary>
/// Editor control for nullable 64-bit signed integer values.
/// </summary>
public class LongEditor : EditorBase<long?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LongEditor"/> class.
    /// </summary>
    public LongEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultLongValueConverter();
}

/// <summary>
/// Editor control for nullable 64-bit unsigned integer values.
/// </summary>
public class ULongEditor : EditorBase<ulong?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ULongEditor"/> class.
    /// </summary>
    public ULongEditor() => ValueConverter ??= ValueConverterFactory.CreateDefaultULongValueConverter();
}
