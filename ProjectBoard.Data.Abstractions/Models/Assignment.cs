using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.Data.Abstractions.Models;

public class Assignment : IDocument
{
    public Assignment()
    {
        this.Id = Guid.NewGuid().ToString();
    }
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DeveloperId { get; set; }
    public AssignmentStatus Status { get; set; }
}
