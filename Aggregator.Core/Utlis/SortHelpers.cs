using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aggregator.Core.Domain;

namespace Aggregator.Core.Utils;

public static class SortHelpers
{
    public static IEnumerable<AggregatedItem> ApplySort(
        IEnumerable<AggregatedItem> items,
        string? sortBy,
        string? order)
    {
        bool desc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
        return (sortBy?.ToLowerInvariant()) switch
        {
            "title" => desc ? items.OrderByDescending(i => i.Title) : items.OrderBy(i => i.Title),
            "date" or "publishedat" => desc ? items.OrderByDescending(i => i.PublishedAt) : items.OrderBy(i => i.PublishedAt),
            _ => desc ? items.OrderByDescending(i => i.PublishedAt) : items.OrderBy(i => i.PublishedAt),
        };
    }
}