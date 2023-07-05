namespace ProjectBoard.API.Features.Responses.Base;

public class PagedDataResponse<T> : DataResponse<List<T>> where T : class
{
    public PagedDataResponse(List<T> payload, PageMetadata pageInfo, ResponseStatus status) : base(payload, status)
    {
        PageInfo = pageInfo;
    }
    public PageMetadata PageInfo { get; set; }
}
