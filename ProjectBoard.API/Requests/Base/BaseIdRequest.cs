using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API.Requests.Base;

public class BaseIdRequest : BaseRequest
{
    public string Id { get; set; }
}
