using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Projects.Models;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.Data.Abstractions;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Projects.Handlers;
public class GetAllProjectsHandler : IRequestHandler<GetAllProjectsRequest, IResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private IIdentity _identity;
    
    public GetAllProjectsHandler(IProjectRepository projectRepository,
                                 IMapper mapper,
                                 IIdentity identity)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _identity = identity;
    }
    public async Task<IResult> Handle(GetAllProjectsRequest request, CancellationToken cancellationToken)
    {
        PaginationResult<Project> projectDb = await _projectRepository.GetPaginatedProjects(request.PageSize,
                                                                                            request.NextPageKey);

        var projectResponse = new List<ProjectDetailsModel>();
        foreach (var project in projectDb.Items)
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
        return Response.OkPage(projectResponse, request.PageSize, projectDb.NextPageKey);        
    }
}


