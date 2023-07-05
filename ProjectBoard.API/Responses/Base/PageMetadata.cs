namespace ProjectBoard.API.Features.Responses.Base;

public class PageMetadata
{
    public int PageSize { get; set; }
    public string? NextPageKey { get; set; } = string.Empty;
}