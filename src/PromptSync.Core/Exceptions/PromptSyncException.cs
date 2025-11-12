namespace PromptSync.Core.Exceptions;

/// <summary>
/// Base exception for all PromptSync errors.
/// </summary>
public class PromptSyncException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptSyncException"/> class.
    /// </summary>
    public PromptSyncException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptSyncException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public PromptSyncException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptSyncException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PromptSyncException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
