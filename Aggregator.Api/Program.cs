using Aggregator.Api.Providers;
using Aggregator.Api.Services;
using Aggregator.Core.Abstractions;
using Aggregator.Core.Domain;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<AggregationService>();

builder.Services.AddHttpClient<IExternalProvider, OpenMeteoProvider>();
builder.Services.AddHttpClient<IExternalProvider, SpaceflightNewsProvider>();
builder.Services.AddHttpClient<IExternalProvider, HackerNewsProvider>();
builder.Services.AddSingleton<ApiStatistics>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<OpenMeteoProvider>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
