using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Interface for encrypting and decrypting sensitive prompts.
/// Single Responsibility: Secure prompt encryption/decryption.
/// </summary>
public interface IEncryptor
{
    /// <summary>
    /// Encrypts a prompt's content.
    /// </summary>
    /// <param name="prompt">The prompt to encrypt.</param>
    /// <param name="password">The encryption password.</param>
    /// <returns>The encrypted prompt.</returns>
    Task<Prompt> EncryptAsync(Prompt prompt, string password);

    /// <summary>
    /// Decrypts a prompt's content.
    /// </summary>
    /// <param name="encryptedPrompt">The encrypted prompt.</param>
    /// <param name="password">The decryption password.</param>
    /// <returns>The decrypted prompt.</returns>
    Task<Prompt> DecryptAsync(Prompt encryptedPrompt, string password);

    /// <summary>
    /// Checks if a prompt is encrypted.
    /// </summary>
    /// <param name="prompt">The prompt to check.</param>
    /// <returns>True if encrypted, false otherwise.</returns>
    bool IsEncrypted(Prompt prompt);

    /// <summary>
    /// Checks if content is encrypted.
    /// </summary>
    /// <param name="content">The content to check.</param>
    /// <returns>True if encrypted, false otherwise.</returns>
    bool IsContentEncrypted(string content);

    /// <summary>
    /// Decrypts a prompt if it's safe to do so (passes security checks).
    /// </summary>
    /// <param name="encryptedPrompt">The encrypted prompt.</param>
    /// <param name="password">The decryption password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with the decrypted prompt if safe.</returns>
    Task<DnaResult> DecryptIfSafeAsync(
        Prompt encryptedPrompt,
        string password,
        CancellationToken cancellationToken = default);
}
