using ProjectBoard.API.Requests.Base;
using System.Text.Json.Serialization;

namespace ProjectBoard.API.Features.TeamMembers.Requests;

public class RemoveTeamMemberRequest : BaseRequest, IRemoveOperation
{
    public string TeamId { get; set; }
    [JsonIgnore]
    public string Id { get; set; }
}
