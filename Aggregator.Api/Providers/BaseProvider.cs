using System.Diagnostics;
using Aggregator.Core.Abstractions;
using Aggregator.Core.Domain;

namespace Aggregator.Api.Providers;

public abstract class BaseProvider : IExternalProvider
{
    private readonly ApiStatistics _stats;

    protected BaseProvider(ApiStatistics stats)
    {
        _stats = stats;
    }

    public abstract string Name { get; }

    public async Task<IReadOnlyList<AggregatedItem>> FetchAsync(AggregationContext ctx, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await FetchItemsAsync(ctx, ct); 
        }
        finally
        {
            sw.Stop();
            _stats.Record(Name, sw.ElapsedMilliseconds);
        }
    }

    protected abstract Task<IReadOnlyList<AggregatedItem>> FetchItemsAsync(AggregationContext ctx, CancellationToken ct);
}
