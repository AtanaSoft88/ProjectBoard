using Amazon.DynamoDBv2.DataModel;
using ProjectBoard.Data.Abstractions.Models;

namespace ProjectBoard.Data.DynamoModels;

[DynamoDBTable("Projects")]
public class DynamoProject : DynamoDocument
{
    [DynamoDBProperty]
    public string Name { get; set; } = null!;
    [DynamoDBProperty]
    public string Description { get; set; } = null!;
    [DynamoDBProperty]
    public string ProjectManagerId { get; set; } = null!;
    [DynamoDBProperty]
    public string? TeamId { get; set; }
    [DynamoDBProperty]
    public List<Assignment> Assignments { get; set; } = new List<Assignment>();
    [DynamoDBProperty]
    public string Status { get; set; }
}
