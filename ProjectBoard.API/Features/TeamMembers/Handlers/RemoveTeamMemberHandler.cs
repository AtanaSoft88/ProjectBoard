using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.TeamMembers.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.TeamMembers.Handlers;

public class RemoveTeamMemberHandler : IRequestHandler<RemoveTeamMemberRequest, IResult>
{
    private readonly IMapper _mapper;
    private readonly ITeamRepository _teamRepository;
    private readonly IIdentity _identity;
        
    public RemoveTeamMemberHandler(IMapper mapper, ITeamRepository teamRepository, IIdentity identity)
    {
        _mapper = mapper;
        _teamRepository = teamRepository;
        _identity = identity;
    }


    public async Task<IResult> Handle(RemoveTeamMemberRequest request, CancellationToken cancellationToken)
    {
        List<User> users = new List<User>();

        Team team = await _teamRepository.GetById(request.TeamId);

        if (team is null)
        {
            return Response.NotFound(ErrorMessages.TeamNotFound, request.TeamId);
        }

        User user = await _identity.SearchUserById(request.Id);

        if (user is null)
        {
            return Response.NotFound(ErrorMessages.UserIdNotFound, request.Id);
        }

        if (!team.DeveloperIds.Contains(user.Id))
        {
            return Response.NotFound(ErrorMessages.NotExistingMember);
        }
        team.DeveloperIds.Remove(user.Id);

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
