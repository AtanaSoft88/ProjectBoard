namespace ProjectBoard.Data.Abstractions.Models;

public class Team : IDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = String.Empty;

    public List<string> DeveloperIds { get; set; } = new List<string>();

    public string TeamLeadId { get; set; } = String.Empty;
}

