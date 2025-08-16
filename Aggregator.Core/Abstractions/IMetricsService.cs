using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Core.Abstractions;

public interface IMetricsService
{
    void Record(string apiName, TimeSpan elapsed, bool success);
    MetricsSnapshot Snapshot();
    ProviderStats GetProviderStats(string apiName);
}

public sealed record MetricsSnapshot(IReadOnlyDictionary<string, ProviderStats> Providers);

public sealed record ProviderStats(
    long TotalRequests,
    TimeSpan AverageResponseTime,
    long SuccessCount,
    long FailureCount,
    BucketCounts Buckets
);

public sealed record BucketCounts(long Fast, long Average, long Slow);