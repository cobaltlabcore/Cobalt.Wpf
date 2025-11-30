using System;
using System.Collections.Generic;
using System.Linq;

namespace Cobalt.WpfUi.Core;

/// <summary>
/// Defines a contract for collecting and managing messages with different severity levels.
/// This interface extends <see cref="IEnumerable{T}"/> to provide enumeration capabilities over collected messages.
/// </summary>
/// <remarks>
/// This interface is designed to provide a thread-safe way to collect diagnostic messages, warnings, and errors
/// during application execution. It supports categorization by severity and optional exception attachment.
/// </remarks>
public interface IMessageCollector : IEnumerable<Message>
{
    /// <summary>
    /// Adds a message to the collection with the specified severity level.
    /// </summary>
    /// <param name="severity">The severity level of the message.</param>
    /// <param name="text">The descriptive text of the message.</param>
    /// <param name="exception">An optional exception associated with the message. Defaults to null.</param>
    void Add(Severity severity, string text, Exception? exception = null);

    /// <summary>
    /// Gets a value indicating whether the collection contains any messages with <see cref="Severity.Error"/> severity.
    /// </summary>
    /// <value>True if there are error messages; otherwise, false.</value>
    bool HasErrors { get; }

    /// <summary>
    /// Gets a value indicating whether the collection contains any messages with <see cref="Severity.Warning"/> severity.
    /// </summary>
    /// <value>True if there are warning messages; otherwise, false.</value>
    bool HasWarnings { get; }
}

/// <summary>
/// Defines the severity levels for messages in the message collection system.
/// </summary>
/// <remarks>
/// This enumeration provides a standardized way to categorize messages by their importance level,
/// from informational notices to critical errors that require attention.
/// </remarks>
public enum Severity
{
    /// <summary>
    /// Informational message that provides general information about the application's operation.
    /// These messages are typically used for logging normal application flow and debugging purposes.
    /// </summary>
    Info,

    /// <summary>
    /// Warning message that indicates a potential issue or unexpected condition that doesn't prevent
    /// the application from continuing but may require attention or could lead to problems.
    /// </summary>
    Warning,

    /// <summary>
    /// Error message that indicates a serious problem or failure condition that prevents normal
    /// operation or could cause the application to fail or produce incorrect results.
    /// </summary>
    Error
}

/// <summary>
/// Represents a diagnostic message with an associated severity level, descriptive text, and optional exception.
/// This is an immutable record-like class that encapsulates all information about a single message entry.
/// </summary>
/// <remarks>
/// <para>
/// This class uses the primary constructor pattern (C# 12.0+) to provide a concise way to create message instances.
/// All properties are read-only after construction, making instances immutable and thread-safe for reading.
/// </para>
/// <para>
/// The class is designed to be lightweight and efficient for scenarios where many message instances may be created
/// during application execution, such as logging, validation, or error reporting scenarios.
/// </para>
/// </remarks>
/// <param name="severity">The severity level of the message (Info, Warning, or Error).</param>
/// <param name="text">The descriptive text content of the message.</param>
/// <param name="exception">An optional exception associated with the message, typically used for error messages.</param>
public class Message(Severity severity, string text, Exception? exception = null)
{
    /// <summary>
    /// Gets the severity level of this message.
    /// </summary>
    /// <value>A <see cref="Core.Severity"/> value indicating the importance level of this message.</value>
    public Severity Severity { get; } = severity;

    /// <summary>
    /// Gets the descriptive text content of this message.
    /// </summary>
    /// <value>A string containing the human-readable description of the message.</value>
    public string Text { get; } = text;

    /// <summary>
    /// Gets the exception associated with this message, if any.
    /// </summary>
    /// <value>An <see cref="Exception"/> instance if the message is related to an exception; otherwise, null.</value>
    public Exception? Exception { get; } = exception;
}

/// <summary>
/// A thread-safe implementation of <see cref="IMessageCollector"/> that stores messages in an internal list.
/// This class provides concurrent access to message collection operations while maintaining data integrity.
/// </summary>
/// <remarks>
/// <para>
/// This implementation uses internal locking to ensure thread safety across all operations. Multiple threads
/// can safely add messages, check for errors/warnings, and enumerate the collection concurrently.
/// </para>
/// <para>
/// The enumerator returned by <see cref="GetEnumerator"/> operates on a snapshot of the collection at the time
/// of enumeration, preventing modification exceptions during iteration while allowing continued collection updates.
/// </para>
/// </remarks>
public class MessageCollector : IMessageCollector
{
    private readonly List<Message> _messages = [];
    private readonly object _lock = new();

    /// <summary>
    /// Gets the total number of messages currently in the collection.
    /// This operation is thread-safe and provides an accurate count at the time of the call.
    /// </summary>
    /// <value>The number of messages in the collection.</value>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _messages.Count;
            }
        }
    }

    /// <summary>
    /// Adds a new message to the collection with the specified severity level.
    /// This operation is thread-safe and can be called concurrently from multiple threads.
    /// </summary>
    /// <param name="severity">The severity level of the message.</param>
    /// <param name="text">The descriptive text of the message.</param>
    /// <param name="exception">An optional exception associated with the message.</param>
    public void Add(Severity severity, string text, Exception? exception = null)
    {
        lock (_lock)
        {
            _messages.Add(new Message(severity, text, exception));
        }
    }

    /// <summary>
    /// Removes all messages from the collection.
    /// This operation is thread-safe and can be called concurrently with other operations.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _messages.Clear();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the collection contains any messages with <see cref="Severity.Error"/> severity.
    /// This operation is thread-safe and provides an accurate result at the time of the call.
    /// </summary>
    /// <value>True if there are error messages in the collection; otherwise, false.</value>
    public bool HasErrors
    {
        get
        {
            lock (_lock)
            {
                return _messages.Any(m => m.Severity == Severity.Error);
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the collection contains any messages with <see cref="Severity.Warning"/> severity.
    /// This operation is thread-safe and provides an accurate result at the time of the call.
    /// </summary>
    /// <value>True if there are warning messages in the collection; otherwise, false.</value>
    public bool HasWarnings
    {
        get
        {
            lock (_lock)
            {
                return _messages.Any(m => m.Severity == Severity.Warning);
            }
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the message collection.
    /// The enumerator operates on a snapshot of the collection, allowing safe enumeration
    /// even while other threads continue to modify the collection.
    /// </summary>
    /// <returns>An enumerator for the message collection.</returns>
    /// <remarks>
    /// This method creates a copy of the internal list to ensure that enumeration is not affected
    /// by concurrent modifications to the collection. This provides thread safety but may have
    /// performance implications for very large collections.
    /// </remarks>
    public IEnumerator<Message> GetEnumerator()
    {
        lock (_lock)
        {
            return _messages.ToList().GetEnumerator();
        }
    }

    /// <summary>
    /// Returns a non-generic enumerator that iterates through the message collection.
    /// </summary>
    /// <returns>A non-generic enumerator for the collection.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Provides extension methods for <see cref="IMessageCollector"/> to simplify adding messages with specific severity levels.
/// These methods offer a more convenient and readable API for common message collection scenarios.
/// </summary>
/// <remarks>
/// <para>
/// This static class contains extension methods that eliminate the need to specify the <see cref="Severity"/> parameter
/// explicitly when adding messages to a collector. This results in cleaner, more expressive code.
/// </para>
/// <para>
/// All extension methods are thin wrappers around the <see cref="IMessageCollector.Add"/> method and maintain
/// the same thread-safety guarantees as the underlying implementation.
/// </para>
/// </remarks>
public static class MessageCollectorExtensions
{
    /// <summary>
    /// Adds an informational message to the collector.
    /// </summary>
    /// <param name="collector">The message collector to add the message to.</param>
    /// <param name="text">The descriptive text of the informational message.</param>
    /// <remarks>
    /// This is a convenience method equivalent to calling <c>collector.Add(Severity.Info, text)</c>.
    /// </remarks>
    public static void AddInfo(this IMessageCollector collector, string text)
        => collector.Add(Severity.Info, text);

    /// <summary>
    /// Adds a warning message to the collector.
    /// </summary>
    /// <param name="collector">The message collector to add the message to.</param>
    /// <param name="text">The descriptive text of the warning message.</param>
    /// <remarks>
    /// This is a convenience method equivalent to calling <c>collector.Add(Severity.Warning, text)</c>.
    /// </remarks>
    public static void AddWarning(this IMessageCollector collector, string text)
        => collector.Add(Severity.Warning, text);

    /// <summary>
    /// Adds an error message to the collector.
    /// </summary>
    /// <param name="collector">The message collector to add the message to.</param>
    /// <param name="text">The descriptive text of the error message.</param>
    /// <remarks>
    /// This is a convenience method equivalent to calling <c>collector.Add(Severity.Error, text)</c>.
    /// </remarks>
    public static void AddError(this IMessageCollector collector, string text)
        => collector.Add(Severity.Error, text);

    /// <summary>
    /// Adds an error message to the collector with an associated exception.
    /// </summary>
    /// <param name="collector">The message collector to add the message to.</param>
    /// <param name="text">The descriptive text of the error message.</param>
    /// <param name="exception">The exception associated with this error message.</param>
    /// <remarks>
    /// This is a convenience method equivalent to calling <c>collector.Add(Severity.Error, text, exception)</c>.
    /// This overload is particularly useful when capturing and logging exceptions that occur during application execution.
    /// </remarks>
    public static void AddError(this IMessageCollector collector, string text, Exception exception)
        => collector.Add(Severity.Error, text, exception);
}