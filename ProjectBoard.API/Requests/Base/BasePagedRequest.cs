using System.ComponentModel;

namespace ProjectBoard.API.Requests.Base;

public class BasePagedRequest : BaseRequest
{
    [DefaultValue(Constants.DefaultPageSize)]
    public int PageSize { get; set; } = Constants.DefaultPageSize;

    public string? NextPageKey { get; set; }
}