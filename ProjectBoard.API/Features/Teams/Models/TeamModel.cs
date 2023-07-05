namespace ProjectBoard.API.Features.Teams.Responses;

public class TeamModel
{
    public string Id { get; set; } = String.Empty;

    public string Name { get; set; } = String.Empty;

    public List<string> DeveloperIds { get; set; } = new List<string>();

    public string TeamLeadId { get; set; } = String.Empty;
}

