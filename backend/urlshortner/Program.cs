using Carter;
using Microsoft.EntityFrameworkCore;
using urlshortner.Persistence;
using urlshortner.Persistence.Context;
using urlshortner.Persistence.Interface;
using urlshortner.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IAnalyticRepository, AnalyticsRepository>();
builder.Services.AddDbContext<UrlDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMemoryCache();

builder.Services.AddCarter();

builder.Services.AddScoped<UrlService>();
builder.Services.AddScoped<AnalyticService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder
                .WithOrigins("https://trimurl.in", "https://www.trimurl.in", "http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed((host) => 
                {
                    Console.WriteLine($"CORS Origin Check: {host}");
                    return true;
                });
        });
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UrlDbContext>();
    // Check if there are any pending migrations
    var pendingMigrations = dbContext.Database.GetPendingMigrations();
    if (pendingMigrations.Any())
    {
        dbContext.Database.Migrate();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.MapCarter();

app.Run();