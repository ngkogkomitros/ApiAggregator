using System.Net.Http.Json;
using Aggregator.Core.Abstractions;
using Aggregator.Core.Domain;

namespace Aggregator.Api.Providers;

public sealed class HackerNewsProvider : BaseProvider
{
    private readonly HttpClient _http;

    public HackerNewsProvider(HttpClient http, ApiStatistics stats) : base(stats)
    {
        _http = http;
    }

    public override string Name => "HackerNews";

    protected override async Task<IReadOnlyList<AggregatedItem>> FetchItemsAsync(AggregationContext ctx, CancellationToken ct)
    {
        // Fetch top story IDs
        var topIds = await _http.GetFromJsonAsync<int[]>("https://hacker-news.firebaseio.com/v0/topstories.json", ct) ?? Array.Empty<int>();
        var limitedIds = topIds.Take(ctx.Limit ?? 20);

        var items = new List<AggregatedItem>();

        foreach (var id in limitedIds)
        {
            var story = await _http.GetFromJsonAsync<HnStory>($"https://hacker-news.firebaseio.com/v0/item/{id}.json", ct);
            if (story == null) continue;

            var item = new AggregatedItem
            {
                Id = story.id.ToString(),
                Source = Name,
                Title = story.title,
                Url = story.url,
                Summary = null,
                Category = "news",
                PublishedAt = story.time.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(story.time.Value)
                    : null,
                Raw = null
            };

            if (ctx.From is not null && (item.PublishedAt < ctx.From)) continue;
            if (ctx.To is not null && (item.PublishedAt > ctx.To)) continue;

            items.Add(item);
        }

        return items;
    }

    private sealed class HnStory
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string? url { get; set; }
        public long? time { get; set; } 
    }
}
