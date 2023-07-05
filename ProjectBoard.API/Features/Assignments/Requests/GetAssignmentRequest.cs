using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API.Features.Assignments.Requests;

public class GetAssignmentRequest : BaseIdRequest, IGetOperation
{
    public string ProjectId { get; set; } = string.Empty;
}
