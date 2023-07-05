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

public class UpdateAssignmentHandler : IRequestHandler<UpdateAssignmentRequest, IResult>
{
    private readonly IMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IExecutionContext _executionContext;

    public UpdateAssignmentHandler(IMapper mapper, IProjectRepository projectRepository, ITeamRepository teamRepository, IExecutionContext executionContext)
    {
        _mapper = mapper;
        _projectRepository = projectRepository;
        _teamRepository = teamRepository;
        _executionContext = executionContext;
    }

    public async Task<IResult> Handle(UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        Project project = await _projectRepository.GetSingle(request.ProjectId);
        if (project is null)
        {
            return Response.BadRequest(ErrorMessages.ProjectNotFound, request.ProjectId);
        }

        Assignment? assignmentToUpdate = project.Assignments.FirstOrDefault(a => a.Id.Equals(request.Id));
        if (assignmentToUpdate is null)
        {
            return Response.BadRequest(ErrorMessages.AssignmentNotFound, request.Id);
        }

        string accessorId = _executionContext.GetCurrentIdentity()!.UserId!;

        bool isAccessorProjectManager = project.ProjectManagerId == accessorId;

        Team? team = project.TeamId is not null
            ? await _teamRepository.GetSingle(project.TeamId)
            : null;

        bool isAccessorTeamLeader = IsUserTeamLeader(accessorId, team);
        bool isTaskAssignedToAccessor = assignmentToUpdate.DeveloperId == accessorId;

        if (!isAccessorProjectManager && !isAccessorTeamLeader && !isTaskAssignedToAccessor)
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

        assignmentToUpdate.Name = request.Name;
        assignmentToUpdate.Description = request.Description;
        assignmentToUpdate.DeveloperId = request.DeveloperId;
        assignmentToUpdate.Status = request.Status;
       
        project.Assignments.Remove(assignmentToUpdate);
        project.Assignments.Add(assignmentToUpdate);

        Project updatedDbProject = await _projectRepository.Update(project);
        Assignment dbAssignment = updatedDbProject.Assignments.First(a => a.Id == assignmentToUpdate.Id);
        AssignmentModel assignmentResponse = _mapper.Map<AssignmentModel>(dbAssignment);
        return Response.OkData(assignmentResponse);
    }

    private bool IsUserTeamMember(string id, Team? team)
    {
        if (team is null) { return false; }
        return team.DeveloperIds.Any(a => a.Equals(id));
    }

    private bool IsUserTeamLeader(string id, Team? team)
    {
        if (team is null || string.IsNullOrEmpty(team.TeamLeadId)) { return false; }
        return team.TeamLeadId!.Equals(id);
    }
}
