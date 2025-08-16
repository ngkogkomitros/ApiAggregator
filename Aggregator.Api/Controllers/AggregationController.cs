using Aggregator.Api.Services;
using Aggregator.Core.Abstractions;
using Aggregator.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Api.Controllers;

[ApiController]
[Route("api/aggregate")]
public class AggregationController : ControllerBase
{
    private readonly AggregationService _service;
    private readonly ApiStatistics _stats;

    public AggregationController(AggregationService service, ApiStatistics stats)
    {
        _service = service;
        _stats = stats;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int? limit,
        CancellationToken ct)
    {
        var ctx = new AggregationContext
        {
            From = from,
            To = to,
            Limit = limit
        };

        var items = await _service.GetAggregatedDataAsync(ctx, ct);
        return Ok(items);
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        var stats = _stats.GetStats();
        return Ok(stats);
    }
}
