using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API.Features.TeamMembers.Requests;

public class CreateTeamMemberRequest : BaseRequest
{
    public string TeamId { get; set; }
    public string UserId { get; set; }
}
