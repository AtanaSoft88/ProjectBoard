using ProjectBoard.API.Requests.Base;
using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.API.Features.Assignments.Requests;

public class CreateAssignmentRequest : BaseRequest
{
    public string ProjectId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DeveloperId { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.AwaitingProgress;
}
