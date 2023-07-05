using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Http;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;

namespace ProjectBoard.API.Features.Assignments.Handlers;

public class CreateAssignmentHandler : IRequestHandler<CreateAssignmentRequest, IResult>
{
    private readonly IMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IExecutionContext _executionContext;

    public CreateAssignmentHandler(IMapper mapper, IProjectRepository projectRepository, ITeamRepository teamRepository, IExecutionContext executionContext)
    {
        _mapper = mapper;
        _projectRepository = projectRepository;
        _teamRepository = teamRepository;
        _executionContext = executionContext;
    }

    public async Task<IResult> Handle(CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        Project project = await _projectRepository.GetSingle(request.ProjectId);
        if (project is null)
        {
            return Response.BadRequest(ErrorMessages.ProjectNotFound, request.ProjectId);
        }

        string accessorId = _executionContext.GetCurrentIdentity()!.UserId!;
        
        bool isAccessorProjectManager = project.ProjectManagerId == accessorId;

        Team team = project.TeamId is not null ?
            await _teamRepository.GetSingle(project.TeamId)
            : null;

        bool isAccessorTeamMember = IsUserTeamMember(accessorId, team);

        if (!isAccessorProjectManager && !isAccessorTeamMember)
        {
            return Results.Unauthorized();
        }

        if (!string.IsNullOrEmpty(request.DeveloperId))
        {
            if (!IsUserTeamMember(request.DeveloperId, team))
            {
                return Response.BadRequest(ErrorMessages.UserIsNotTeamMember, request.DeveloperId);
            }
        }

        Assignment assignment = _mapper.Map<Assignment>(request);
        project.Assignments.Add(assignment);
        Project updatedDbProject = await _projectRepository.Update(project);
        Assignment dbAssignment = updatedDbProject.Assignments.First(a => a.Id == assignment.Id);
        AssignmentModel assignmentResponse = _mapper.Map<AssignmentModel>(dbAssignment);
        return Response.OkData(assignmentResponse);
    }

    private bool IsUserTeamMember(string id, Team team)
    {
        if (team is null) { return false; }
        return team.DeveloperIds.Any(a => a.Equals(id));
    }
}
