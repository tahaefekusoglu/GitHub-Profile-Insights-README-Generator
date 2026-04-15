using System.Threading.RateLimiting;
using GitHubReadmeGenerator.API.Services;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = (Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")
    ?? builder.Configuration["AllowedOrigins"]
    ?? "http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var isExplicitProduction =
    string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Production",
        StringComparison.OrdinalIgnoreCase) ||
    string.Equals(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"), "Production",
        StringComparison.OrdinalIgnoreCase);

if (isExplicitProduction)
{
    var hasLocalhostOrigin = allowedOrigins.Any(o =>
        o.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase) ||
        o.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase));

    if (hasLocalhostOrigin)
        Console.WriteLine(
            "WARNING: ALLOWED_ORIGINS contains a localhost entry in a Production environment. " +
            "Set the ALLOWED_ORIGINS environment variable to your production frontend URL.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("ip", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("github", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("User-Agent", "GitHubReadmeGenerator");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromMilliseconds(500);
});

builder.Services.AddMemoryCache();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddSingleton<ClaudeService>();
builder.Services.AddSingleton<OpenAiService>();
builder.Services.AddSingleton<GeminiService>();
builder.Services.AddSingleton<IAiService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var provider = AiProviderFactory.GetActiveProvider(config);
    return provider switch
    {
        "claude" => sp.GetRequiredService<ClaudeService>(),
        "openai" => sp.GetRequiredService<OpenAiService>(),
        "gemini" => sp.GetRequiredService<GeminiService>(),
        _ => sp.GetRequiredService<ClaudeService>()
    };
});
builder.Services.AddSingleton<IReadmeTemplateService, ReadmeTemplateService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
