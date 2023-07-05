using ProjectBoard.API.Features.Projects.Models;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.API.Features.Assignments.Models;

public class AssignmentDetailsModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UserModel? Developer { get; set; }
    public AssignmentStatus Status { get; set; }
    public ProjectInfoModel ProjectDetails { get; set; } = new();
}

