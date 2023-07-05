using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Responses;
using ProjectBoard.API.Http;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Enums;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Projects.Handlers;
public class UpdateProjectHandler : IRequestHandler<UpdateProjectRequest, IResult>
{
    private readonly IProjectRepository _repository;
    private readonly IMapper _mapper;
    private readonly IIdentity _identity;
    private readonly IExecutionContext _executionContext;

    public UpdateProjectHandler(IProjectRepository repository,
                                IMapper mapper,
                                IIdentity identity,
                                IExecutionContext executionContext)
    {
        _repository = repository;
        _mapper = mapper;
        _identity = identity;
        _executionContext = executionContext;
    }
    public async Task<IResult> Handle(UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        Project project = await _repository.GetSingle(request.Id);
        if (project == null)
        {
            return Response.NotFound(ErrorMessages.ProjectNotFoundById);
        }
        User? databaseUser = await _identity.SearchUserById(request.ProjectManagerId);
        CurrentUser? cuerrentLoggedUser = _executionContext.GetCurrentIdentity();
        if (databaseUser is null || databaseUser.Id != cuerrentLoggedUser?.UserId)
        {
            return Response.NotFound(ErrorMessages.PorjectManagerMismatch);
        }
        Project projectToUpdate = _mapper.Map<Project>(request);
        projectToUpdate.Assignments = project.Assignments;

        bool areAllTasksDone = projectToUpdate.Assignments.Any()
                             ? projectToUpdate.Assignments.All(x => x.Status == AssignmentStatus.Done)
                             : false;
        projectToUpdate.Status = areAllTasksDone ? ProjectStatus.Done : request.Status;
        
        bool isProjectStatusAcceptable = true;
        if (areAllTasksDone && request.Status == ProjectStatus.Approved)
        {
            projectToUpdate.Status = ProjectStatus.Approved;
        }        
        else if(!areAllTasksDone && (request.Status == ProjectStatus.Approved || request.Status == ProjectStatus.Done))
        {
            isProjectStatusAcceptable = false;
            projectToUpdate.Status = ProjectStatus.InProgress;
        }
        else
        {
            projectToUpdate.Status = request.Status;
        }        
        if (isProjectStatusAcceptable == false)
        {            
            return Response.BadRequest(ErrorMessages.ProjectNotFinished);
        }
        Project updatedProject = await _repository.Update(projectToUpdate);
        ProjectModel projectResult = _mapper.Map<ProjectModel>(updatedProject);
        return Response.OkData(projectResult);
    }
}
