using System.Text.Json;
using Carter;
using Carter.OpenApi;
using Microsoft.AspNetCore.Mvc;
using urlshortner.Dto.Url;
using urlshortner.Request;
using urlshortner.Services;

namespace urlshortner.Modules;

public class ShortnerModule : CarterModule
{
    public ShortnerModule(): base("/api")
    {
       WithTags("Url Shortner");
       IncludeInOpenApi();
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/short", async (
            [FromBody] UrlRequestDto request,
            UrlService urlService
        ) =>
        {
            var response = await urlService.ShortenUrlAsync(request.LongUrl);
            return response.StatusCode switch
            {
                200 => Results.Ok(response),
                400 => Results.BadRequest(response),
                _ => Results.Problem(response.ErrorMessage)
            };
        });
        
        app.MapGet("/urls", async (
            UrlService urlService
        ) =>
        {
            var response = await urlService.GetUrlsAsync();
            return Results.Ok(response);
        });
        
        app.MapGet("/paged-urls", async (
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromQuery] string? searchTerm,
            UrlService urlService
        ) =>
        {
            var pagedUrls = await urlService.GetPagedUrlAsync(pageIndex, pageSize, searchTerm);
            return Results.Ok(pagedUrls);
        });
    }
}