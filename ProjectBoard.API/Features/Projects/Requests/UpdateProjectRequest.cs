using ProjectBoard.API.Requests.Base;
using System.Text.Json.Serialization;

namespace ProjectBoard.API.Features.Projects.Requests;

public class UpdateProjectRequest : CreateProjectRequest, IUpdateOperation
{
    [JsonIgnore]
    public string Id { get; set; } = string.Empty;
}
