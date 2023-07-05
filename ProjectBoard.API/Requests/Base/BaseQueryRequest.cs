namespace ProjectBoard.API.Requests.Base;

public class BaseQueryRequest : BasePagedRequest, ISearchOperation
{
    public string Query { get; set; }
}