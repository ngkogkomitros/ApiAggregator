using System.Collections.Concurrent;

namespace Aggregator.Core.Domain;

public sealed class ApiStatistics
{
    private readonly ConcurrentDictionary<string, List<long>> _responseTimes = new();

    public void Record(string providerName, long milliseconds)
    {
        var list = _responseTimes.GetOrAdd(providerName, _ => new List<long>());
        lock (list) { list.Add(milliseconds); }
    }

    public IReadOnlyDictionary<string, ApiStatsResult> GetStats()
    {
        return _responseTimes.ToDictionary(
            kvp =>
            {
                var count = kvp.Value.Count;
                var avg = kvp.Value.Count > 0 ? kvp.Value.Average() : 0;
                var buckets = new
                {
                    Fast = kvp.Value.Count(t => t < 100),
                    Average = kvp.Value.Count(t => t >= 100 && t <= 200),
                    Slow = kvp.Value.Count(t => t > 200)
                };

                return kvp.Key;
            },
            kvp => new ApiStatsResult
            {
                TotalRequests = kvp.Value.Count,
                AverageResponseMs = kvp.Value.Count > 0 ? (long)kvp.Value.Average() : 0,
                FastCount = kvp.Value.Count(t => t < 100),
                AverageCount = kvp.Value.Count(t => t >= 100 && t <= 200),
                SlowCount = kvp.Value.Count(t => t > 200)
            });
    }
}

public sealed class ApiStatsResult
{
    public int TotalRequests { get; set; }
    public long AverageResponseMs { get; set; }
    public int FastCount { get; set; }
    public int AverageCount { get; set; }
    public int SlowCount { get; set; }
}
