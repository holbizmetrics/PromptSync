namespace PromptSync.Core.Models;

/// <summary>
/// Represents a workflow chain that composes multiple prompts.
/// </summary>
public record WorkflowChain
{
    /// <summary>
    /// Gets the unique identifier for the workflow.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the name of the workflow.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description of what the workflow does.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the ordered list of workflow steps.
    /// </summary>
    public required IReadOnlyList<WorkflowStep> Steps { get; init; }

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets a value indicating whether the workflow is enabled.
    /// </summary>
    public bool IsEnabled { get; init; } = true;
}

/// <summary>
/// Represents a single step in a workflow chain.
/// </summary>
public record WorkflowStep
{
    /// <summary>
    /// Gets the step identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the step name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the type of step (prompt, dna_iterate, harvest, etc.).
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets the configuration for this step.
    /// </summary>
    public required IReadOnlyDictionary<string, object> Configuration { get; init; }

    /// <summary>
    /// Gets the order index of this step.
    /// </summary>
    public required int Order { get; init; }
}

/// <summary>
/// Represents the result of executing a workflow.
/// </summary>
public record WorkflowResult
{
    /// <summary>
    /// Gets a value indicating whether the workflow executed successfully.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the results from each step.
    /// </summary>
    public required IReadOnlyDictionary<string, object> StepResults { get; init; }

    /// <summary>
    /// Gets the final output of the workflow.
    /// </summary>
    public object? FinalOutput { get; init; }

    /// <summary>
    /// Gets the error message if the workflow failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the execution duration.
    /// </summary>
    public TimeSpan ExecutionDuration { get; init; }
}
