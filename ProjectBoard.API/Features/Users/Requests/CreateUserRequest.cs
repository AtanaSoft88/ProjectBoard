using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API.Features.Users.Requests
{
    public class CreateUserRequest : BaseRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
