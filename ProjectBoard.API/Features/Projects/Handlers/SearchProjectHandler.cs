using MediatR;
using ProjectBoard.API.Features.Projects.Requests;
using AutoMapper;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Projects.Models;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Projects.Handlers;

public class SearchProjectHandler : IRequestHandler<SearchProjectRequest, IResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IIdentity _identity;

    public SearchProjectHandler(IProjectRepository projectRepository, IMapper mapper, IIdentity identity)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _identity = identity;
    }

    public async Task<IResult> Handle(SearchProjectRequest request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.SearchProjectByName(request.Query, request.PageSize, request.NextPageKey!);

        var projectResponse = new List<ProjectDetailsModel>();
        foreach (var project in projects.Items)
        {
            User projectManager = await _identity.SearchUserById(project.ProjectManagerId);
            List<AssignmentModel> assignments = _mapper.Map<List<AssignmentModel>>(project.Assignments);
            var currentProject = new ProjectDetailsModel
            {
                Id = project.Id,
                Name = project.Name,
                ProjectManager = _mapper.Map<UserModel>(projectManager),
                Status = project.Status,
                Assignments = assignments,
                Description = project.Description,
                TeamId = project.TeamId,
            };
            projectResponse.Add(currentProject);
        }
        return Response.OkPage(projectResponse, request.PageSize, projects.NextPageKey);
    }
}
