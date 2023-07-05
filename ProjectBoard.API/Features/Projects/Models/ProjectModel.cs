using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.Data.Abstractions.Enums;
namespace ProjectBoard.API.Features.Projects.Responses;
public class ProjectModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ProjectManagerId { get; set; } = string.Empty;
    public List<AssignmentModel> Assignments { get; set; } = new List<AssignmentModel>();
    public string? TeamId { get; set; }
}

