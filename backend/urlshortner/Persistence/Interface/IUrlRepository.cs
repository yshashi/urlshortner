using urlshortner.Models;

namespace urlshortner.Persistence.Interface;

public interface IUrlRepository
{
    Task<bool> SaveUrlAsync(Url url);
    Task<Url?> GetUrlByCodeAsync(string urlCode);
    
    Task<Url?> GetUrlByLongUrl(string longUrl);
    
    Task<IReadOnlyCollection<Url>> GetUrlsAsync();
    
    Task<PagedResult<Url>> GetPagedUrlAsync(int pageIndex, int pageSize, string? searchTerm);
}

public class PagedResult<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageIndex > 1;
    
    public bool HasNextPage => PageIndex < TotalPages;
    public List<T> Data { get; set; } = [];
}