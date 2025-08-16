using Aggregator.Core.Abstractions;
using Aggregator.Core.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Aggregator.Api.Services;

public sealed class AggregationService
{
    private readonly IEnumerable<IExternalProvider> _providers;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AggregationService> _logger;
    private readonly ApiStatistics _stats;

    public AggregationService(
        IEnumerable<IExternalProvider> providers,
        IMemoryCache cache,
        ILogger<AggregationService> logger,
        ApiStatistics stats)
    {
        _providers = providers;
        _cache = cache;
        _logger = logger;
        _stats = stats;
    }

    public async Task<IReadOnlyList<AggregatedItem>> GetAggregatedDataAsync(AggregationContext ctx, CancellationToken ct)
    {
        var cacheKey = $"aggregated:{ctx.From}-{ctx.To}-{ctx.Limit}";

        if (_cache.TryGetValue(cacheKey, out List<AggregatedItem> cachedItems))
        {
            _logger.LogInformation("Returning cached results for key {CacheKey}", cacheKey);

            return cachedItems;
        }

        var allItems = new List<AggregatedItem>();

        foreach (var provider in _providers)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var items = await provider.FetchAsync(ctx, ct);
                sw.Stop();

                _stats.Record(provider.Name, sw.ElapsedMilliseconds);

                allItems.AddRange(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from provider {Provider}", provider.Name);
            }
        }

        var result = allItems
            .Where(i => (ctx.From == null || i.PublishedAt >= ctx.From) &&
                        (ctx.To == null || i.PublishedAt <= ctx.To))
            .Take(ctx.Limit ?? 20)
            .ToList();

      
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return result;
    }

    public IReadOnlyDictionary<string, ApiStatsResult> GetStats() => _stats.GetStats();
}
