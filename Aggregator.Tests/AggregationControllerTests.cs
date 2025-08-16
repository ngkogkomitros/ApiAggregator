using Aggregator.Api.Controllers;
using Aggregator.Core.Domain;
using Aggregator.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Aggregator.Api.Services;

namespace Aggregator.Tests;

public class AggregationControllerTests
{
    [Fact]
    public async Task GetAggregatedData_ShouldRespectFromToLimit()
    {
        // Arrange
        var stats = new ApiStatistics();

        // Create mock provider
        var mockProvider = new Mock<IExternalProvider>();
        mockProvider.Setup(p => p.Name).Returns("MockProvider");

        var now = DateTimeOffset.UtcNow;

        // 3 mock items with specific PublishedAt
        var items = new[]
        {
            new AggregatedItem { Id = "1", Title = "Test1", PublishedAt = now.AddMinutes(-30) },
            new AggregatedItem { Id = "2", Title = "Test2", PublishedAt = now.AddMinutes(-20) },
            new AggregatedItem { Id = "3", Title = "Test3", PublishedAt = now.AddMinutes(-10) }
        };

        mockProvider.Setup(p => p.FetchAsync(It.IsAny<AggregationContext>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((AggregationContext ctx, CancellationToken ct) =>
                        items.Where(i =>
                            (!ctx.From.HasValue || i.PublishedAt >= ctx.From) &&
                            (!ctx.To.HasValue || i.PublishedAt <= ctx.To)
                        ).ToList()
                    );

        // Create a memory cache
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var logger = Mock.Of<ILogger<AggregationService>>();

        // Create AggregationService with single mock provider
        var aggrService = new AggregationService(
            new[] { mockProvider.Object },
            memoryCache,
            logger,
            stats
        );

        var controller = new AggregationController(aggrService, stats);

       
        var from = now.AddMinutes(-30);
        var to = now.AddMinutes(-15);  
        var limit = 10;

       
        var result = await controller.Get(from, to, limit, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedItems = Assert.IsAssignableFrom<IEnumerable<AggregatedItem>>(okResult.Value);

        Assert.Equal(2, returnedItems.Count());
        Assert.All(returnedItems, i => Assert.InRange(i.PublishedAt.Value, from, to));
    }

    [Fact]
    public void GetStatistics_ShouldReturnStats()
    {
        // Arrange
        var stats = new ApiStatistics();
        stats.Record("TestProvider", 50);
        stats.Record("TestProvider", 150);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var logger = Mock.Of<ILogger<AggregationService>>();

        var aggregationService = new AggregationService(
            Array.Empty<IExternalProvider>(),
            memoryCache,
            logger,
            stats
        );

        var controller = new AggregationController(aggregationService, stats);

        // Act
        var result = controller.GetStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dictionary = Assert.IsType<Dictionary<string, ApiStatsResult>>(okResult.Value);
        Assert.True(dictionary.ContainsKey("TestProvider"));
        Assert.Equal(2, dictionary["TestProvider"].TotalRequests);
    }
}
