using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API.Features.Teams.Requests;

public class CreateTeamRequest : BaseRequest
{

    public string Name { get; set; } = String.Empty;

    public string TeamLeadId { get; set; } = String.Empty;
}

