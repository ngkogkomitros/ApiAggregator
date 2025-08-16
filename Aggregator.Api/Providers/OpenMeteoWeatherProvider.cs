using System.Diagnostics;
using Aggregator.Core.Abstractions;
using Aggregator.Core.Domain;
using Aggregator.Api.Models;

namespace Aggregator.Api.Providers;
public sealed class OpenMeteoProvider : BaseProvider
{
    private readonly HttpClient _httpClient;

    public OpenMeteoProvider(HttpClient httpClient, ApiStatistics stats)
        : base(stats)
    {
        _httpClient = httpClient;
    }

    public override string Name => "OpenMeteo";

    protected override async Task<IReadOnlyList<AggregatedItem>> FetchItemsAsync(AggregationContext ctx, CancellationToken ct)
    {
        var url = "https://api.open-meteo.com/v1/forecast?latitude=35&longitude=139&hourly=temperature_2m";
        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, ct) ?? new OpenMeteoResponse();

        var items = response.Hourly?.Time?.Select((t, i) => new AggregatedItem
        {
            Id = $"{i}",
            Source = Name,
            Title = $"Temp at {t}",
            Summary = $"{response.Hourly.Temperature2m?[i]}°C",
            PublishedAt = DateTimeOffset.TryParse(t, out var dt) ? dt : null,
            Raw = null
        }) ?? Enumerable.Empty<AggregatedItem>();

        if (ctx.From is not null) items = items.Where(i => i.PublishedAt >= ctx.From);
        if (ctx.To is not null) items = items.Where(i => i.PublishedAt <= ctx.To);

        return items.Take(ctx.Limit ?? 20).ToList();
    }
}


public class OpenMeteoResponse
{
    public HourlyData? Hourly { get; set; }
}

public class HourlyData
{
    public List<string>? Time { get; set; }
    public List<double>? Temperature2m { get; set; }
}
