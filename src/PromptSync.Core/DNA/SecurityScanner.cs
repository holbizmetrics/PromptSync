using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Minimal security scanner that looks for high-risk patterns in prompt content.
/// This implementation is conservative and intended as a foundation for more
/// sophisticated scanners (static rules + AI-driven analysis).
/// </summary>
public class SecurityScanner : ISecurityScanner
{
    private readonly ILogger<SecurityScanner> _logger;

    public SecurityScanner(ILogger<SecurityScanner> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DnaResult> ScanAsync(Prompt prompt, CancellationToken cancellationToken = default)
    {
        if (prompt == null) throw new ArgumentNullException(nameof(prompt));
        var result = await ScanContentAsync(prompt.Content ?? string.Empty, cancellationToken).ConfigureAwait(false);
        return result with { Prompt = prompt };
    }

    public Task<DnaResult> ScanContentAsync(string content, CancellationToken cancellationToken = default)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));

        _logger.LogInformation("Scanning content for security issues");

        var vulnerabilities = new List<SecurityVulnerability>();
        var riskScore = 0;

        // Detect potentially dangerous commands or code execution requests
        if (content.Contains("rm -rf", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("format C:", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("shutdown", StringComparison.OrdinalIgnoreCase))
        {
            vulnerabilities.Add(new SecurityVulnerability
            {
                Type = "DangerousCommand",
                Severity = SecuritySeverity.Critical,
                Description = "Prompt contains potentially dangerous system commands.",
                Recommendation = "Remove or neutralize destructive system commands before executing."
            });
            riskScore += 80;
        }

        // Detect requests for secrets or credentials
        if (Regex.IsMatch(content, "(password|secret|api[_-]?key|token)", RegexOptions.IgnoreCase))
        {
            vulnerabilities.Add(new SecurityVulnerability
            {
                Type = "SecretLeak",
                Severity = SecuritySeverity.High,
                Description = "Prompt requests or contains secrets/credentials.",
                Recommendation = "Avoid including secrets in prompts. Use secure storage and parameters instead."
            });
            riskScore += 60;
        }

        // Detect instructions to bypass security
        if (Regex.IsMatch(content, "(bypass|disable|ignore)\\s+(security|auth|validation)", RegexOptions.IgnoreCase))
        {
            vulnerabilities.Add(new SecurityVulnerability
            {
                Type = "BypassSecurity",
                Severity = SecuritySeverity.High,
                Description = "Prompt attempts to bypass security or authentication.",
                Recommendation = "Do not execute prompts that instruct bypassing security."
            });
            riskScore += 70;
        }

        // Simple heuristics for sensitive data patterns (emails, SSNs)
        if (Regex.IsMatch(content, "\\b\\d{3}-\\d{2}-\\d{4}\\b") ||
            Regex.IsMatch(content, "[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,}", RegexOptions.IgnoreCase))
        {
            vulnerabilities.Add(new SecurityVulnerability
            {
                Type = "SensitiveData",
                Severity = SecuritySeverity.Medium,
                Description = "Prompt contains patterns that look like sensitive personal data.",
                Recommendation = "Redact personal data and avoid including it in prompts."
            });
            riskScore += 40;
        }

        var riskLevel = SecurityRiskLevel.Safe;
        if (riskScore >= 150) riskLevel = SecurityRiskLevel.Critical;
        else if (riskScore >= 100) riskLevel = SecurityRiskLevel.High;
        else if (riskScore >= 60) riskLevel = SecurityRiskLevel.Moderate;
        else if (riskScore >= 20) riskLevel = SecurityRiskLevel.Low;

        var result = new DnaResult
        {
            Success = true,
            Content = content,
            SecurityRiskScore = Math.Min(riskScore, 100),
            RiskLevel = riskLevel,
            Vulnerabilities = vulnerabilities
        };

        return Task.FromResult(result);
    }

    public async Task<bool> IsSafeAsync(Prompt prompt, CancellationToken cancellationToken = default)
    {
        if (prompt == null) throw new ArgumentNullException(nameof(prompt));
        var r = await ScanAsync(prompt, cancellationToken).ConfigureAwait(false);
        return r.RiskLevel == SecurityRiskLevel.Safe || r.RiskLevel == SecurityRiskLevel.Low;
    }
}

