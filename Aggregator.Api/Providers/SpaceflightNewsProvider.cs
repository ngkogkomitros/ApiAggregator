using Aggregator.Api.Providers;
using Aggregator.Core.Abstractions;
using Aggregator.Core.Domain;

public sealed class SpaceflightNewsProvider : BaseProvider
{
    private readonly HttpClient _http;

    public SpaceflightNewsProvider(HttpClient http, ApiStatistics stats) : base(stats)
    {
        _http = http;
    }

    public override string Name => "Spaceflight";

    protected override async Task<IReadOnlyList<AggregatedItem>> FetchItemsAsync(AggregationContext ctx, CancellationToken ct)
    {
        var res = await _http.GetFromJsonAsync<ArticlesResponse>("https://api.spaceflightnewsapi.net/v4/articles/", ct) ?? new ArticlesResponse();

        var items = res.results.Select(a => new AggregatedItem
        {
            Id = a.id.ToString(),
            Source = Name,
            Title = a.title,
            Summary = a.summary,
            Url = a.url,
            Category = "news",
            PublishedAt = DateTimeOffset.TryParse(a.published_at, out var dt) ? dt : null,
            Raw = null
        });

        if (ctx.From is not null) items = items.Where(i => i.PublishedAt >= ctx.From);
        if (ctx.To is not null) items = items.Where(i => i.PublishedAt <= ctx.To);

        return items.Take(ctx.Limit ?? 20).ToList();
    }

    private sealed class ArticlesResponse
    {
        public List<Article> results { get; set; } = new();
    }

    private sealed class Article
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string? summary { get; set; }
        public string url { get; set; } = string.Empty;
        public string published_at { get; set; } = string.Empty;
    }
}
