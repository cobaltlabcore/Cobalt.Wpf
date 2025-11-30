using Cobalt.WpfUi.Core.Converters;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System;

namespace Cobalt.WpfUi.Controls.Editors;

/// <summary>
/// Generic base class for editors that handle strongly-typed values.
/// Provides two-way binding between text representation and typed values using value converters.
/// </summary>
/// <typeparam name="T">The type of value handled by this editor.</typeparam>
public abstract class EditorBase<T> : EditorBase
{
    /// <summary>
    /// Indicates whether the Value property should be updated immediately when text changes.
    /// </summary>
    private bool _updateValueWhenTextChanged;

    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        name: nameof(Value),
        propertyType: typeof(T),
        ownerType: typeof(EditorBase<T>),
        typeMetadata: new FrameworkPropertyMetadata(
            defaultValue: default(T),
            flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            propertyChangedCallback: OnValuePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="ValueConverter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueConverterProperty = DependencyProperty.Register(
        name: nameof(ValueConverter),
        propertyType: typeof(IValueConverter<T>),
        ownerType: typeof(EditorBase<T>),
        typeMetadata: new PropertyMetadata(null, OnValueConverterPropertyChanged));

    /// <summary>
    /// Identifies the <see cref="ValueFormat"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueFormatProperty = DependencyProperty.Register(
        name: nameof(ValueFormat),
        propertyType: typeof(string),
        ownerType: typeof(EditorBase<T>),
        typeMetadata: new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the strongly-typed value of the editor.
    /// </summary>
    public T? Value
    {
        get => (T)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the value converter used to convert between text and the typed value.
    /// </summary>
    public IValueConverter<T>? ValueConverter
    {
        get => (IValueConverter<T>)GetValue(ValueConverterProperty);
        set => SetValue(ValueConverterProperty, value);
    }

    /// <summary>
    /// Gets or sets the format string used when converting the value to text.
    /// </summary>
    public string? ValueFormat
    {
        get => (string?)GetValue(ValueFormatProperty);
        set => SetValue(ValueFormatProperty, value);
    }

    /// <summary>
    /// Called when the Value dependency property changes.
    /// </summary>
    /// <param name="d">The dependency object that owns the property.</param>
    /// <param name="e">The event data.</param>
    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((EditorBase<T>)d).OnValueChanged(e);

    /// <summary>
    /// Called when the Value property changes. Updates the text representation.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
    {
        UpdateText();
    }

    /// <summary>
    /// Called when the ValueConverter dependency property changes.
    /// </summary>
    /// <param name="d">The dependency object that owns the property.</param>
    /// <param name="e">The event data.</param>
    private static void OnValueConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((EditorBase<T>)d).OnValueConverterChanged(e);

    /// <summary>
    /// Called when the ValueConverter property changes. Override in derived classes to handle converter changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnValueConverterChanged(DependencyPropertyChangedEventArgs e)
    { }

    /// <summary>
    /// Called when the text content changes. Updates the value if immediate updates are enabled.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);

        if (_updateValueWhenTextChanged)
            UpdateValue();
    }

    /// <summary>
    /// Clears the editor by setting the value to its default and updating the text.
    /// </summary>
    protected override void ClearEditor()
    {
        SetCurrentValue(ValueProperty, default(T));
        UpdateText();
    }

    /// <summary>
    /// Called when the editor loses focus. Updates the value from the current text.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        UpdateValue();
    }

    /// <summary>
    /// Called when the control template is applied. Configures update behavior and synchronizes initial state.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Check if the Value binding uses PropertyChanged trigger for immediate updates
        var binding = BindingOperations.GetBinding(this, ValueProperty);
        if (binding is { UpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged })
        {
            _updateValueWhenTextChanged = true;
        }

        // Synchronize initial state between Text and Value
        if (string.IsNullOrEmpty(Text) && Value is not null)
            UpdateText();
        if (Value is null && !string.IsNullOrEmpty(Text))
            UpdateValue();
    }

    /// <summary>
    /// Updates the Value property from the current text content using the value converter.
    /// Handles null values for nullable and reference types, and manages validation errors.
    /// </summary>
    protected virtual void UpdateValue()
    {
        // Handle empty text for nullable or reference types
        if (string.IsNullOrEmpty(Text))
        {
            var bindingExpression = GetBindingExpression(ValueProperty);
            if (bindingExpression?.ParentBinding?.Path?.Path != null)
            {
                var dataContext = bindingExpression.DataItem;
                if (dataContext != null)
                {
                    var propertyType = GetPropertyType(dataContext, bindingExpression.ParentBinding.Path.Path);
                    if (propertyType != null)
                    {
                        // Allow null for nullable types and reference types
                        if ((propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ||
                            !propertyType.IsValueType)
                        {
                            SetCurrentValue(ValueProperty, default(T));
                            ClearValidationError();
                            return;
                        }
                    }
                }
            }
        }

        // Attempt to parse the text using the value parser
        var (success, value) = ValueConverter?.TryParseValue(Text) ?? (false, default);
        if (success)
        {
            SetCurrentValue(ValueProperty, value);
            ClearValidationError();
        }
        else
            AddValidationError($"Value '{Text}' could not be converted.");
    }

    /// <summary>
    /// Gets the property type from a binding path by traversing the object hierarchy.
    /// </summary>
    /// <param name="obj">The root object.</param>
    /// <param name="propertyPath">The property path to traverse.</param>
    /// <returns>The type of the property at the end of the path, or null if not found.</returns>
    private static Type? GetPropertyType(object obj, string propertyPath)
    {
        var parts = propertyPath.Split('.');
        var currentType = obj.GetType();

        foreach (var part in parts)
        {
            var propertyInfo = currentType.GetProperty(part);
            if (propertyInfo == null)
                return null;

            currentType = propertyInfo.PropertyType;
        }

        return currentType;
    }

    /// <summary>
    /// Updates the text content from the current Value using the value converter and format.
    /// </summary>
    protected virtual void UpdateText()
    {
        var formattedValue = ValueConverter?.FormatValue(Value, ValueFormat);
        SetCurrentValue(TextProperty, formattedValue);
    }

    /// <summary>
    /// Adds a validation error to the Value property binding.
    /// </summary>
    /// <param name="errorMessage">The error message to display.</param>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void AddValidationError(string errorMessage)
    {
        var bindingExpression = GetBindingExpression(ValueProperty);
        if (bindingExpression is null) return;
        var validationError = new ValidationError(new ExceptionValidationRule(), bindingExpression)
        {
            ErrorContent = errorMessage
        };

        Validation.MarkInvalid(bindingExpression, validationError);
    }

    /// <summary>
    /// Clears any validation errors from the Value property binding.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected void ClearValidationError()
    {
        var bindingExpression = GetBindingExpression(ValueProperty);
        if (bindingExpression is not null)
            Validation.ClearInvalid(bindingExpression);
    }
}