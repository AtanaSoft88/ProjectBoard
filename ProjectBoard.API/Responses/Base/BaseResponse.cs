namespace ProjectBoard.API.Features.Responses.Base;

public class BaseResponse
{
    public BaseResponse(ResponseStatus status)
    {
        Status = status;
    }

    public ResponseStatus Status { get; set; }
}
