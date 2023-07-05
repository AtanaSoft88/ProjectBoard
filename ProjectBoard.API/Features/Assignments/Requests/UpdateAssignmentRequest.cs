using ProjectBoard.API.Requests.Base;
using System.Text.Json.Serialization;

namespace ProjectBoard.API.Features.Assignments.Requests;

public class UpdateAssignmentRequest : CreateAssignmentRequest, IUpdateOperation
{
    [JsonIgnore]
    public string Id { get; set; } = string.Empty;
}
