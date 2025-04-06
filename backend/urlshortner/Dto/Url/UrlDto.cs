namespace urlshortner.Request;

public record UrlDto(int Id, string LongUrl, string ShortUrl, string CreatedAt);
