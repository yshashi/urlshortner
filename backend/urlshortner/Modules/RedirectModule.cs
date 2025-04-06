using Carter;
using Microsoft.AspNetCore.Mvc;
using urlshortner.Services;

namespace urlshortner.Modules;

public class RedirectModule : CarterModule
{
    public RedirectModule() : base("/")
    {
        WithTags("URL Redirect");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("{url_code}", async (
            [FromRoute(Name = "url_code")] string urlCode,
            UrlService urlService
        ) =>
        {
            var response = await urlService.GetLongUrlAsync(urlCode);
            return response is null ? Results.NotFound("Url not found") : Results.Redirect(response);
        })
        .WithName("RedirectUrl")
        .WithTags("URL Redirect");
    }
}
