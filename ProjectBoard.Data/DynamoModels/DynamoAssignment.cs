using Amazon.DynamoDBv2.DataModel;
using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.Data.DynamoModels;

public class DynamoAssignment : DynamoDocument
{
    [DynamoDBProperty]
    public string Name { get; set; } = string.Empty;
    [DynamoDBProperty]
    public string Description { get; set; } = string.Empty;
    [DynamoDBProperty]
    public string? DeveloperId { get; set; }
    [DynamoDBProperty]
    public AssignmentStatus Status { get; set; }
}
