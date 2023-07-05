using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API.Features.Assignments.Requests;

public class GetAllAssignmentsRequest : BasePagedRequest
{
    public string ProjectId { get; set; } = string.Empty;
}
