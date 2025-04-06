namespace urlshortner.Request;

public record UrlResponseDto(bool IsSuccess, int StatusCode, string? ShortUrl = null, string? ErrorMessage=null);