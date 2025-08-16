namespace Aggregator.Api.Models;

public sealed record QueryParams
{
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Query { get; init; }
    public int? Limit { get; init; }
    public string? Category { get; init; }
    public string? SortBy { get; init; } 
    public string? Order { get; init; } 
}