using Aggregator.Core.Domain;

namespace Aggregator.Core.Abstractions;

public interface IExternalProvider
{
    string Name { get; }
    Task<IReadOnlyList<AggregatedItem>> FetchAsync(AggregationContext ctx, CancellationToken ct);
}
public sealed record AggregationContext
{
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Category { get; init; }
    public int? Limit { get; init; }
    public string? Query { get; init; }

    public AggregationContext() { }

    public AggregationContext(DateTimeOffset? from, DateTimeOffset? to, string? category, int? limit, string? query)
    {
        From = from;
        To = to;
        Category = category;
        Limit = limit;
        Query = query;
    }
}