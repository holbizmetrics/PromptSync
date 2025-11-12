using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using PromptSync.Core.Models;
using PromptSync.Core.Exceptions;

namespace PromptSync.Core.DNA;

/// <summary>
/// Simple AES-based encryptor for prompt content. This implementation is intentionally
/// minimal and uses a password-derived key. It is suitable for local encryption needs
/// and unit testing, but for production consider using platform-provided secure storage
/// and authenticated encryption schemes with proper key management.
/// </summary>
public class Encryptor : IEncryptor
{
    private readonly ILogger<Encryptor> _logger;

    public Encryptor(ILogger<Encryptor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Prompt> EncryptAsync(Prompt prompt, string password)
    {
        if (prompt == null) throw new ArgumentNullException(nameof(prompt));
        if (password == null) throw new ArgumentNullException(nameof(password));

        var contentBytes = Encoding.UTF8.GetBytes(prompt.Content ?? string.Empty);
        var encrypted = EncryptBytes(contentBytes, password);
        var encryptedContent = Convert.ToBase64String(encrypted);

        var result = prompt with { Content = encryptedContent, IsEncrypted = true };
        return await Task.FromResult(result);
    }

    public async Task<Prompt> DecryptAsync(Prompt encryptedPrompt, string password)
    {
        if (encryptedPrompt == null) throw new ArgumentNullException(nameof(encryptedPrompt));
        if (password == null) throw new ArgumentNullException(nameof(password));

        try
        {
            var bytes = Convert.FromBase64String(encryptedPrompt.Content ?? string.Empty);
            var decrypted = DecryptBytes(bytes, password);
            var content = Encoding.UTF8.GetString(decrypted);
            var result = encryptedPrompt with { Content = content, IsEncrypted = false };
            return await Task.FromResult(result);
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Content is not valid base64 or password is incorrect");
            throw new PromptSyncException("Decryption failed: invalid content or password", ex);
        }
    }

    public bool IsEncrypted(Prompt prompt)
    {
        return prompt?.IsEncrypted ?? false;
    }

    public bool IsContentEncrypted(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return false;
        // Quick base64 heuristic
        try
        {
            Convert.FromBase64String(content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<DnaResult> DecryptIfSafeAsync(Prompt encryptedPrompt, string password, CancellationToken cancellationToken = default)
    {
        if (encryptedPrompt == null) throw new ArgumentNullException(nameof(encryptedPrompt));

        // In a fuller implementation you'd run a security scan before decrypting.
        var decrypted = await DecryptAsync(encryptedPrompt, password);
        return new DnaResult { Success = true, Prompt = decrypted, Content = decrypted.Content };
    }

    private static byte[] EncryptBytes(byte[] plaintext, string password)
    {
        using var aes = Aes.Create();
        var key = DeriveKey(password, aes.KeySize / 8);
        aes.Key = key;
        aes.GenerateIV();
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);
        using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(plaintext, 0, plaintext.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    private static byte[] DecryptBytes(byte[] cipherWithIv, string password)
    {
        using var aes = Aes.Create();
        var ivLength = aes.BlockSize / 8;
        var iv = cipherWithIv.Take(ivLength).ToArray();
        var cipher = cipherWithIv.Skip(ivLength).ToArray();
        var key = DeriveKey(password, aes.KeySize / 8);
        aes.Key = key;
        aes.IV = iv;
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(cipher, 0, cipher.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    private static byte[] DeriveKey(string password, int keyBytes)
    {
        using var kdf = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256);
        return kdf.GetBytes(keyBytes);
    }
}
