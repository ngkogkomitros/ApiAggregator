using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Core.Domain;

public class AggregatedItem
{
    public string? Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Category { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTime? Date { get; set; }
    public object? Raw { get; set; }
}