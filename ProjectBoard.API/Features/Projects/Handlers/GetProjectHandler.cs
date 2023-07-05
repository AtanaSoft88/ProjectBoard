using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Projects.Models;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Projects.Handlers;
public class GetProjectHandler : IRequestHandler<GetProjectRequest, IResult>
{
    private readonly IProjectRepository _repository;
    private readonly IMapper _mapper;
    private readonly IIdentity _identity;

    public GetProjectHandler(IProjectRepository repository,
                             IMapper mapper,
                             IIdentity identity)
    {
        _repository = repository;
        _mapper = mapper;
        _identity = identity;
    }

    public async Task<IResult> Handle(GetProjectRequest request, CancellationToken cancellationToken)
    {
        Project project = await _repository.GetSingle(request.Id);        
        if (project is null)
        {            
            return Response.NotFound(ErrorMessages.ProjectNotFoundById);
        }
        User projectManager = await _identity.SearchUserById(project.ProjectManagerId);
        var projectResponse = new ProjectDetailsModel()
        {
            Id = project.Id,
            Name = project.Name,
            Status = project.Status,
            Assignments = _mapper.Map<List<AssignmentModel>>(project.Assignments),
            Description = project.Description,
            ProjectManager = _mapper.Map<UserModel>(projectManager),
            TeamId = project.TeamId
        };       
       return Response.OkData(projectResponse);
    }
}
