using System.Text;
using Microsoft.Extensions.Caching.Memory;
using urlshortner.Extensions;
using urlshortner.Models;
using urlshortner.Persistence.Interface;
using urlshortner.Request;

namespace urlshortner.Services;

public class UrlService(
    IUrlRepository urlRepository,
    IAnalyticRepository analyticRepository,
    IConfiguration configuration,
    IMemoryCache cache)
{
    private const string URL_CACHE_KEY_PREFIX = "url_";
    private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromMinutes(30);

    public async Task<UrlResponseDto> ShortenUrlAsync(string longUrl)
    {
        var baseUrl = configuration["BaseUrl"];
        
        // Validate the URL
        if (!IsValidUrl(longUrl))
        {
            return new UrlResponseDto(false, 400, null, "Invalid URL");
        }

        // Check cache first for the long URL
        var cacheKey = $"{URL_CACHE_KEY_PREFIX}{longUrl}";
        if (cache.TryGetValue(cacheKey, out Url? cachedUrl))
        {
            return new UrlResponseDto(true, 200, $"{baseUrl}/{cachedUrl?.UrlCode}");
        }

        var url = await urlRepository.GetUrlByLongUrl(longUrl);
        if (url is not null)
        {
            // Cache the result
            cache.Set(cacheKey, url, CACHE_DURATION);
            cache.Set($"{URL_CACHE_KEY_PREFIX}{url.UrlCode}", url, CACHE_DURATION);
            return new UrlResponseDto(true, 200, $"{baseUrl}/{url.UrlCode}");
        }
        
        var urlCode = GenerateUrlCode();
        url = new Url
        {
            LongUrl = longUrl,
            UrlCode = urlCode,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await urlRepository.SaveUrlAsync(url);
        if (!result) return new UrlResponseDto(false, 500, null, "Failed to shorten URL");

        // Cache the new URL
        cache.Set(cacheKey, url, CACHE_DURATION);
        cache.Set($"{URL_CACHE_KEY_PREFIX}{urlCode}", url, CACHE_DURATION);
        return new UrlResponseDto(true, 200, $"{baseUrl}/{urlCode}");
    }
    
    public async Task<string?> GetLongUrlAsync(string urlCode)
    {
        // Check cache first
        var cacheKey = $"{URL_CACHE_KEY_PREFIX}{urlCode}";
        // Will be used to check if the URL is already cached for analytics
        // if (cache.TryGetValue(cacheKey, out Url? cachedUrl))
        // {
        //     if(cachedUrl is not null)
        //     {
        //         await analyticRepository.AddAnalyticAsync(cachedUrl.Id, cachedUrl.UrlCode);
        //     }
        //     return cachedUrl?.LongUrl;
        // }

        var url = await urlRepository.GetUrlByCodeAsync(urlCode);
        if (url is not null)
        {
            // Cache the result
            cache.Set(cacheKey, url, CACHE_DURATION);
            cache.Set($"{URL_CACHE_KEY_PREFIX}{url.LongUrl}", url, CACHE_DURATION);
            await analyticRepository.AddAnalyticAsync(url.Id, url.UrlCode);
        }
        return url?.LongUrl;
    }
    
    public async Task<IReadOnlyCollection<Url>> GetUrlsAsync()
    {
        return (await urlRepository.GetUrlsAsync());
    }
    
    public async Task<PagedResult<UrlDto>> GetPagedUrlAsync(int pageIndex, int pageSize, string? searchTerm)
    {
        var response = await urlRepository.GetPagedUrlAsync(pageIndex, pageSize, searchTerm);
        var baseUrl = configuration["BaseUrl"];
        var urls = response.Data
            .Select(u => new UrlDto(u.Id,
                u.LongUrl,
                $"{baseUrl}/{u.UrlCode}",
                u.CreatedAt.ToDateTimeString()))
            .ToList();
        
        return new PagedResult<UrlDto>
        {
            PageIndex = response.PageIndex,
            PageSize = response.PageSize,
            TotalCount = response.TotalCount,
            Data = urls
        };
    }

    private static string GenerateUrlCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int codeLength = 7;
        var code = new StringBuilder();
        var random = new Random();
        
        for (var i = 0; i < codeLength; i++)
        {
            code.Append(chars[random.Next(chars.Length)]);
        }

        return code.ToString();
    }
    
    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}