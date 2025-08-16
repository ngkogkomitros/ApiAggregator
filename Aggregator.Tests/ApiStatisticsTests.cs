
using Aggregator.Core.Domain;
using Xunit;
namespace Aggregator.Tests;

public class ApiStatisticsTests
{
    [Fact]
    public void Record_ShouldAddResponseTime()
    {
        var stats = new ApiStatistics();

        stats.Record("TestProvider", 120);
        stats.Record("TestProvider", 80);

        var result = stats.GetStats();

        Assert.True(result.ContainsKey("TestProvider"));
        var providerStats = result["TestProvider"];
        Assert.Equal(2, providerStats.TotalRequests);
        Assert.Equal(100, providerStats.AverageResponseMs); 
        Assert.Equal(1, providerStats.FastCount); 
        Assert.Equal(1, providerStats.AverageCount);
        Assert.Equal(0, providerStats.SlowCount);
    }
}
