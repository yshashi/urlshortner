namespace urlshortner.Models;

public class Analytics
{
    public int Id { get; init; }
    public DateTime ClickedTime { get; set; }

    public required string UrlCode { get; init; }
    
    // Foreign Key
    public int UrlId { get; init; }
    
    // Navigation property
    public Url? Url { get; init; }
}