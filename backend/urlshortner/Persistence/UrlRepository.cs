using Microsoft.EntityFrameworkCore;
using urlshortner.Models;
using urlshortner.Persistence.Context;
using urlshortner.Persistence.Interface;

namespace urlshortner.Persistence;

public class UrlRepository(UrlDbContext urlDbContext) : IUrlRepository
{
    public async Task<bool> SaveUrlAsync(Url url)
    {
        await urlDbContext.Urls.AddAsync(url);
        var result = await urlDbContext.SaveChangesAsync();
        return result > 0;
    }

    public async Task<Url?> GetUrlByCodeAsync(string urlCode)
    {
        return await urlDbContext.Urls.FirstOrDefaultAsync(u => u.UrlCode == urlCode);
    }

    public async Task<Url?> GetUrlByLongUrl(string longUrl)
    {
        return await urlDbContext.Urls.FirstOrDefaultAsync(u => u.LongUrl == longUrl);
    }

    public async Task<IReadOnlyCollection<Url>> GetUrlsAsync()
    {
        return await urlDbContext.Urls.ToListAsync();
    }
    
    public async Task<PagedResult<Url>> GetPagedUrlAsync(int pageIndex, int pageSize, string? searchTerm)
    {
        var query = urlDbContext.Urls.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(x => 
                x.LongUrl.ToLower().Contains(searchTerm) || 
                x.UrlCode.ToLower().Contains(searchTerm));
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Url>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalItems,
            Data = items
        };
    }
}