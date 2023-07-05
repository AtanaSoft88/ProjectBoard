using ProjectBoard.API.Requests.Base;
using System.Text.Json.Serialization;

namespace ProjectBoard.API.Features.Teams.Requests;

public class UpdateTeamRequest : CreateTeamRequest, IUpdateOperation
{
    [JsonIgnore]
    public string Id { get; set; } = string.Empty;
}


