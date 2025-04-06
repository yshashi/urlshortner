namespace urlshortner.Models;

public class Url
{
    public int Id { get; init; }
    public required string LongUrl { get; init ; }
    public required string UrlCode { get; init; }
    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<Analytics> Analytics { get; set; } = null!;
}