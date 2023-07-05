using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.Data.Abstractions.Models
{
    public class Project : IDocument
    {        
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProjectManagerId { get; set; } = string.Empty;
        public string? TeamId { get; set; }
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ProjectStatus Status { get; set; }
    }
}
