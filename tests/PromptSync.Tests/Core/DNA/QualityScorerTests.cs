using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PromptSync.Core.DNA;
using PromptSync.Core.Models;
using PromptSync.Core.Services;
using Xunit;

namespace PromptSync.Tests.Core.DNA;

/// <summary>
/// Unit tests for the QualityScorer class.
/// Demonstrates proper testing with mocking and assertions.
/// </summary>
public class QualityScorerTests
{
    private readonly Mock<IAIService> _mockAIService;
    private readonly Mock<ILogger<QualityScorer>> _mockLogger;
    private readonly QualityScorer _scorer;

    public QualityScorerTests()
    {
        _mockAIService = new Mock<IAIService>();
        _mockLogger = new Mock<ILogger<QualityScorer>>();
        _scorer = new QualityScorer(_mockAIService.Object, _mockLogger.Object);
    }

    [Fact]
    public void GetQuickScore_WithEmptyContent_ReturnsZero()
    {
        // Arrange
        var content = string.Empty;

        // Act
        var score = _scorer.GetQuickScore(content);

        // Assert
        score.Should().Be(0.0);
    }

    [Fact]
    public void GetQuickScore_WithBasicPrompt_ReturnsPositiveScore()
    {
        // Arrange
        var content = "Create a detailed analysis of the given data.";

        // Act
        var score = _scorer.GetQuickScore(content);

        // Assert
        score.Should().BeGreaterThan(0.0);
        score.Should().BeLessThanOrEqualTo(10.0);
    }

    [Fact]
    public void GetQuickScore_WithStructuredPrompt_ReturnsHigherScore()
    {
        // Arrange
        var content = @"Analyze the following:
- Data point 1
- Data point 2
- Data point 3

Context: This is important analysis.
Example: For instance, we might see trends.";

        // Act
        var score = _scorer.GetQuickScore(content);

        // Assert
        score.Should().BeGreaterThan(5.0);
        score.Should().BeLessThanOrEqualTo(10.0);
    }

    [Fact]
    public void GetQuickScore_WithVariables_IncludesVariableBonus()
    {
        // Arrange
        var contentWithVariables = "Analyze {{data}} and provide {{output_format}} results.";
        var contentWithoutVariables = "Analyze data and provide results.";

        // Act
        var scoreWith = _scorer.GetQuickScore(contentWithVariables);
        var scoreWithout = _scorer.GetQuickScore(contentWithoutVariables);

        // Assert
        scoreWith.Should().BeGreaterThan(scoreWithout);
    }

    [Fact]
    public async Task ScoreContentAsync_WithValidContent_ReturnsSuccessResult()
    {
        // Arrange
        var content = "Create a comprehensive report on the given topic.";
        
        _mockAIService
            .Setup(x => x.SendStructuredPromptAsync<QualityBreakdown>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QualityBreakdown
            {
                Clarity = 7.5,
                Specificity = 6.0,
                Structure = 5.5,
                Context = 6.5,
                Examples = 4.0
            });

        // Act
        var result = await _scorer.ScoreContentAsync(content);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.QualityScore.Should().BeGreaterThan(0);
        result.QualityDetails.Should().NotBeNull();
        result.QualityDetails!.Clarity.Should().Be(7.5);
    }

    [Fact]
    public async Task ScoreContentAsync_WhenAIServiceFails_FallsBackToQuickScore()
    {
        // Arrange
        var content = "Create a report.";
        
        _mockAIService
            .Setup(x => x.SendStructuredPromptAsync<QualityBreakdown>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("AI service unavailable"));

        // Act
        var result = await _scorer.ScoreContentAsync(content);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.QualityScore.Should().BeGreaterThan(0);
        result.Metadata.Should().ContainKey("fallback_reason");
        result.Metadata["ai_powered"].Should().Be(false);
    }

    [Fact]
    public async Task ScoreAsync_WithNullPrompt_ThrowsArgumentNullException()
    {
        // Arrange
        Prompt? prompt = null;

        // Act
        Func<Task> act = async () => await _scorer.ScoreAsync(prompt!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ScoreContentAsync_WithInvalidContent_ThrowsArgumentException(string? content)
    {
        // Act
        Func<Task> act = async () => await _scorer.ScoreContentAsync(content!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
