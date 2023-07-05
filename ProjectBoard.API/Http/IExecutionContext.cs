namespace ProjectBoard.API.Http;

public interface IExecutionContext
{
    CurrentUser? GetCurrentIdentity();    
}
