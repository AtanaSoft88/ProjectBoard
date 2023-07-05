using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.TeamMembers.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.TeamMembers.Handlers;

public class CreateTeamMemberHandler : IRequestHandler<CreateTeamMemberRequest, IResult>
{
    private readonly IMapper _mapper;
    private readonly ITeamRepository _teamRepository;
    private readonly IIdentity _identity;

    public CreateTeamMemberHandler(IMapper mapper, ITeamRepository teamRepository, IIdentity identity)
    {
        _mapper = mapper;
        _teamRepository = teamRepository;
        _identity = identity;
    }

    public async Task<IResult> Handle(CreateTeamMemberRequest request, CancellationToken cancellationToken)
    {
        List<User> users = new List<User>();

        Team team = await _teamRepository.GetById(request.TeamId);

        if (team is null)
        {
            return Response.NotFound(ErrorMessages.TeamNotFound, request.TeamId);
        }

        User user = await _identity.SearchUserById(request.UserId);

        if (user is null)
        {
           
            return Response.NotFound(ErrorMessages.UserIdNotFound,request.UserId);
        }

        if (team.DeveloperIds.Contains(user.Id))
        {
            return Response.BadRequest(ErrorMessages.ExistingMember);
        }
        team.DeveloperIds.Add(user.Id);

        await _teamRepository.Update(team);

        foreach (string id in team.DeveloperIds)
        {
            User loopUser = await _identity.SearchUserById(id);
            users.Add(loopUser);
        }

        List<UserModel> members = _mapper.Map<List<UserModel>>(users);

        return Response.OkData(members);
    }
}
