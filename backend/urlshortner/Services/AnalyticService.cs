using urlshortner.Persistence.Interface;

namespace urlshortner.Services;

public class AnalyticService(IAnalyticRepository analyticRepository)
{
    public async Task<int> GetTotalClicksAsync(string urlCode)
    {
        return await analyticRepository.GetTotalClicksAsync(urlCode);
    }
}