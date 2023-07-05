using MediatR;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Projects.Models;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Assignments.Handlers;

public class GetAllAssignmentsHandler : IRequestHandler<GetAllAssignmentsRequest, IResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly  IIdentity _usersRepository;

    public GetAllAssignmentsHandler(IProjectRepository projectRepository, IIdentity usersRepository)
    {
        _projectRepository = projectRepository;
        _usersRepository = usersRepository;
    }

    public async Task<IResult> Handle(GetAllAssignmentsRequest request, CancellationToken cancellationToken)
    {
        Project project = await _projectRepository.GetSingle(request.ProjectId);
        if (project is null)
        {
            return Response.NotFound(ErrorMessages.ProjectNotFound, request.ProjectId);
        }

        if (string.IsNullOrEmpty(request.NextPageKey))
        {
            request.NextPageKey = "1";
        }

        int countToSkip = (int.Parse(request.NextPageKey) - 1) * request.PageSize;

        List<AssignmentDetailsModel> assignments = project.Assignments.Select(x => new AssignmentDetailsModel
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Developer = !string.IsNullOrEmpty(x.DeveloperId) ? new UserModel { Id = x.DeveloperId } : null,
            Status = x.Status,
            ProjectDetails = new ProjectInfoModel
            {
                Id = project.Id,
                Name = project.Name,
            }
        })
            .Skip(countToSkip)
            .Take(request.PageSize)
            .ToList();

        foreach (AssignmentDetailsModel assignment in assignments)
        {
            if (assignment.Developer is not null)
            {
                User currentDeveloper = await _usersRepository.SearchUserById(assignment.Developer.Id);
                if (currentDeveloper is not null)
                {
                    assignment.Developer.Username = currentDeveloper.Username;
                    assignment.Developer.Email = currentDeveloper.Email;
                }
            }
        }

        int totalAssignmentsCount = project.Assignments.Count;
        int totalPages = (int)Math.Ceiling((double)totalAssignmentsCount / request.PageSize);
        int? updatedNextPageKey = int.Parse(request.NextPageKey) + 1;
        if (updatedNextPageKey > totalPages)
        {
            updatedNextPageKey = null;
        }

        return Response.OkPage(assignments, request.PageSize, updatedNextPageKey?.ToString());
    }
}
