namespace ProjectBoard.API.Features.Responses.Base;

public class DataResponse<T> : BaseResponse
{
    public DataResponse(T payload, ResponseStatus status) : base(status)
    {
        Payload = payload;
    }
    public T Payload { get; set; }

}