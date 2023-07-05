using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.API.Features.Assignments.Models;

public class AssignmentModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DeveloperId { get; set; }
    public AssignmentStatus Status { get; set; }
}
