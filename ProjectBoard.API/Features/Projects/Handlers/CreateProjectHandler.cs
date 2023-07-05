using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Responses;
using ProjectBoard.API.Http;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Projects.Handlers;
public class CreateProjectHandler : IRequestHandler<CreateProjectRequest, IResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IIdentity _identity;
    private readonly IExecutionContext _executionContext;
    public CreateProjectHandler(IProjectRepository projectRepository,
                                IMapper mapper,
                                IExecutionContext executionContext,
                                IIdentity identity)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _executionContext = executionContext;
        _identity = identity;
    }

    public async Task<IResult> Handle(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        User? databaseUser = await _identity.SearchUserById(request.ProjectManagerId);
        CurrentUser? cuerrentLoggedUser = _executionContext.GetCurrentIdentity();
        if (databaseUser is null || databaseUser.Id != cuerrentLoggedUser?.UserId)
        {
            return Response.NotFound(ErrorMessages.PorjectManagerMismatch);
        }
        var project = new Project
        {
            Name = request.Name,
            Status = request.Status,
            Description = request.Description,
            ProjectManagerId = request.ProjectManagerId!,
            TeamId = request.TeamId,
        };
        Project projectResponse = await _projectRepository.Create(project);
        ProjectModel projectModel = _mapper.Map<ProjectModel>(projectResponse);
        return Response.OkData(projectModel);
    }
}
