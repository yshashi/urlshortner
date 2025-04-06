using Microsoft.EntityFrameworkCore;
using urlshortner.Models;
using urlshortner.Persistence.Context;
using urlshortner.Persistence.Interface;

namespace urlshortner.Persistence;

public class AnalyticsRepository(UrlDbContext context) : IAnalyticRepository
{
    public async Task<Analytics> AddAnalyticAsync(int urlId, string urlCode)
    {
        var analytic = new Analytics()
        {
            ClickedTime = DateTime.UtcNow,
            UrlId = urlId,
            UrlCode = urlCode
        };
        
        await context.Analytics.AddAsync(analytic);
        await context.SaveChangesAsync();
        return analytic;
    }

    public async Task<int> GetTotalClicksAsync(string urlCode)
    {
        var count = await context.Analytics.CountAsync(a => a.UrlCode == urlCode);
        return count;
    }
}