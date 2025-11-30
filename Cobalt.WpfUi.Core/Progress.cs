using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cobalt.WpfUi.Core;

/// <summary>
/// Defines a contract for tracking and reporting progress information.
/// </summary>
/// <remarks>
/// This interface provides a standardized way to communicate progress updates
/// including numeric progress values, descriptive messages, and indeterminate states.
/// It's commonly used for long-running operations, file transfers, and background tasks.
/// </remarks>
public interface IProgress
{
    /// <summary>
    /// Gets or sets the current progress value as a percentage (0.0 to 100.0).
    /// </summary>
    /// <value>
    /// A double value representing the progress percentage. Typically ranges from 0.0 (no progress)
    /// to 100.0 (complete), but the exact range depends on the implementation.
    /// </value>
    double Value { get; set; }
    
    /// <summary>
    /// Gets or sets an optional descriptive message about the current progress state.
    /// </summary>
    /// <value>
    /// A string describing what operation is currently being performed, or null if no message is available.
    /// Examples might include "Loading files...", "Processing data...", or "Almost finished...".
    /// </value>
    string? Message { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the progress is indeterminate (unknown duration).
    /// </summary>
    /// <value>
    /// True if the progress duration cannot be determined (showing a spinner or pulsing bar);
    /// false if the progress can be measured with a specific value.
    /// </value>
    bool IsIndeterminate { get; set; }
    
    /// <summary>
    /// Updates one or more progress properties atomically.
    /// </summary>
    /// <param name="value">The new progress value, or null to leave unchanged.</param>
    /// <param name="message">The new progress message, or null to leave unchanged.</param>
    /// <param name="isIndeterminate">The new indeterminate state, or null to leave unchanged.</param>
    /// <remarks>
    /// This method provides a convenient way to update multiple progress properties
    /// in a single call, which can be more efficient than setting properties individually
    /// when multiple changes need to be made simultaneously.
    /// </remarks>
    void UpdateProgress(double? value = null, string? message = null, bool? isIndeterminate = null);
}

/// <summary>
/// Provides a concrete implementation of <see cref="IProgress"/> with automatic property change notifications.
/// </summary>
/// <remarks>
/// <para>
/// This class implements both <see cref="IProgress"/> for progress tracking and <see cref="INotifyPropertyChanged"/>
/// for automatic UI data binding support. It's particularly useful in MVVM applications where progress
/// information needs to be displayed in the user interface and automatically updated when values change.
/// </para>
/// <para>
/// The class uses a custom <see cref="SetField{T}"/> method that implements the standard property
/// change notification pattern, ensuring that UI elements bound to these properties are automatically
/// updated when values change.
/// </para>
/// </remarks>
public class Progress : IProgress, INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the current progress value as a percentage.
    /// </summary>
    /// <value>
    /// A double value representing the progress percentage. Setting this property
    /// will automatically raise the <see cref="PropertyChanged"/> event if the value changes.
    /// </value>
    public double Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }
    private double _value;
    
    /// <summary>
    /// Gets or sets an optional descriptive message about the current progress state.
    /// </summary>
    /// <value>
    /// A string describing the current operation, or null if no message is set.
    /// Setting this property will automatically raise the <see cref="PropertyChanged"/> event if the value changes.
    /// </value>
    public string? Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }
    private string? _message;
    
    /// <summary>
    /// Gets or sets a value indicating whether the progress is indeterminate.
    /// </summary>
    /// <value>
    /// True for indeterminate progress (unknown duration); false for determinate progress with measurable value.
    /// Setting this property will automatically raise the <see cref="PropertyChanged"/> event if the value changes.
    /// </value>
    public bool IsIndeterminate
    {
        get => _isIndeterminate;
        set => SetField(ref _isIndeterminate, value);
    }
    private bool _isIndeterminate;

    /// <summary>
    /// Updates one or more progress properties atomically with automatic property change notifications.
    /// </summary>
    /// <param name="value">The new progress value, or null to leave the current value unchanged.</param>
    /// <param name="message">The new progress message, or null to leave the current message unchanged.</param>
    /// <param name="isIndeterminate">The new indeterminate state, or null to leave the current state unchanged.</param>
    /// <remarks>
    /// This method provides an efficient way to update multiple properties at once.
    /// Each parameter that is not null will trigger a property change notification if the value
    /// actually changes from the current value. This can be more efficient than setting
    /// properties individually when multiple updates are needed.
    /// </remarks>
    public void UpdateProgress(double? value = null, string? message = null, bool? isIndeterminate = null)
    {
        if (value is not null)
            Value = value.Value;
        if (message is not null)
            Message = message;
        if (isIndeterminate is not null)
            IsIndeterminate = isIndeterminate.Value;
    }
    
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <remarks>
    /// This event is part of the <see cref="INotifyPropertyChanged"/> interface and is automatically
    /// raised by the <see cref="SetField{T}"/> method when property values change. UI frameworks
    /// like WPF use this event to automatically update bound controls when the underlying data changes.
    /// </remarks>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that changed. This parameter is automatically provided by the
    /// <see cref="CallerMemberNameAttribute"/> when called from a property setter.
    /// </param>
    /// <remarks>
    /// This method is typically called by the <see cref="SetField{T}"/> method and property setters
    /// to notify listeners that a property value has changed. The <see cref="CallerMemberNameAttribute"/>
    /// ensures that the correct property name is passed automatically when called from property setters.
    /// </remarks>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets a property value and raises the <see cref="PropertyChanged"/> event if the value has changed.
    /// This is a utility method that implements the standard property setter pattern with change notification.
    /// </summary>
    /// <typeparam name="T">The type of the property being set.</typeparam>
    /// <param name="field">A reference to the backing field for the property.</param>
    /// <param name="value">The new value to assign to the field.</param>
    /// <param name="propertyName">
    /// The name of the property being set. This parameter is automatically provided by the
    /// <see cref="CallerMemberNameAttribute"/> when called from a property setter.
    /// </param>
    /// <returns>
    /// True if the field value was changed and the <see cref="PropertyChanged"/> event was raised;
    /// false if the value was already equal to the new value and no change occurred.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method uses <see cref="EqualityComparer{T}.Default"/> to determine if the value has
    /// actually changed before updating the field and raising the property changed event.
    /// This prevents unnecessary UI updates and ensures efficient data binding performance.
    /// </para>
    /// <para>
    /// Example usage in a property setter:
    /// <code>
    /// public string MyProperty
    /// {
    ///     get => _myProperty;
    ///     set => SetField(ref _myProperty, value);
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}