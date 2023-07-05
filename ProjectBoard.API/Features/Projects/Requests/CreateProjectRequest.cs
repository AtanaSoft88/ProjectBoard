using ProjectBoard.API.Requests.Base;
using ProjectBoard.Data.Abstractions.Enums;
namespace ProjectBoard.API.Features.Projects.Requests;
public class CreateProjectRequest : BaseRequest
{    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProjectManagerId { get; set; } = string.Empty;
    public string? TeamId { get; set; }   
    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
}
