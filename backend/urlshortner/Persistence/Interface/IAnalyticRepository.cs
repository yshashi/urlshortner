using urlshortner.Dto.Analytics;
using urlshortner.Models;

namespace urlshortner.Persistence.Interface;

public interface IAnalyticRepository
{
    Task<Analytics> AddAnalyticAsync(int urlId, string urlCode);
    
    Task<int> GetTotalClicksAsync(string urlCode);

}