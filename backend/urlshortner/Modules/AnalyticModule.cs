using Carter;
using Microsoft.AspNetCore.Mvc;
using urlshortner.Services;

namespace urlshortner.Modules;

public class AnalyticModule : CarterModule
{
    public AnalyticModule() : base("/api")
    {
        WithTags("Url Analytic");
        IncludeInOpenApi();
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/analytics/{url_code}", async (
            [FromRoute(Name = "url_code")] string urlCode,
            AnalyticService analyticService
        ) =>
        {
            var totalClicks = await analyticService.GetTotalClicksAsync(urlCode);
            return Results.Ok(new
            {
                TotalClicks = totalClicks
            });
        });
    }
}