using ProjectBoard.API.Features.Responses.Base.Enums;

namespace ProjectBoard.API.Features.Responses.Base;

public class ResponseStatus
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public Reason Reason { get; set; }

    public static ResponseStatus Error(string message)
    {
        return new ResponseStatus() { IsSuccess = false, Message = message };
    }

    public static ResponseStatus Success()
    {
        return new ResponseStatus() { IsSuccess = true };
    }
}