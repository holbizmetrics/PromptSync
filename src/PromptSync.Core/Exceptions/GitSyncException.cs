namespace PromptSync.Core.Exceptions;

/// <summary>
/// Exception thrown when Git synchronization operations fail.
/// </summary>
public class GitSyncException : PromptSyncException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitSyncException"/> class.
    /// </summary>
    public GitSyncException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitSyncException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public GitSyncException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitSyncException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public GitSyncException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
