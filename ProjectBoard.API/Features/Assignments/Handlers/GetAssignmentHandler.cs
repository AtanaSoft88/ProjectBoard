using AutoMapper;
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

public class GetAssignmentHandler : IRequestHandler<GetAssignmentRequest, IResult>
{
    private readonly IMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly IIdentity _usersRepository;

    public GetAssignmentHandler(IMapper mapper, IProjectRepository projectRepository, IIdentity usersRepository)
    {
        _mapper = mapper;
        _projectRepository = projectRepository;
        _usersRepository = usersRepository;
    }

    public async Task<IResult> Handle(GetAssignmentRequest request, CancellationToken cancellationToken)
    {
        Project project = await _projectRepository.GetSingle(request.ProjectId);
        if (project is null)
        {
            return Response.NotFound(ErrorMessages.ProjectNotFound, request.ProjectId);
        }

        Assignment? assignment = project.Assignments.FirstOrDefault(x => x.Id == request.Id);
        if (assignment is null)
        {
            return Response.NotFound(ErrorMessages.AssignmentNotFound, request.Id);
        }

        User? developer = null;
        if (assignment.DeveloperId is not null)
        {
            developer = await _usersRepository.SearchUserById(assignment.DeveloperId);
        }

        UserModel? developerInfomationModel = _mapper.Map<UserModel?>(developer);

        AssignmentDetailsModel assignmentModel = new()
        {
            Id = assignment.Id,
            Name = assignment.Name,
            Description = assignment.Description,
            Developer = developerInfomationModel,
            Status = assignment.Status,
            ProjectDetails = new ProjectInfoModel
            {
                Id = project.Id,
                Name = project.Name,
            }
        };

        return Response.OkData(assignmentModel);
    }
}
