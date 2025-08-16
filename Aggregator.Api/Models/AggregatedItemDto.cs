using Aggregator.Core.Domain;

namespace Aggregator.Api.Models;

public static class AggregatedItemDto
{
    public static object From(AggregatedItem x) => new
    {
        x.Id,
        x.Title,
        x.Summary,
        x.Url,
        x.Source,
        x.Category,
        PublishedAt = x.PublishedAt?.UtcDateTime,
    };
}