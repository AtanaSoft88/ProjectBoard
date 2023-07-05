using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Teams.Models;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Teams.Handlers;

public class GetTeamHandler : IRequestHandler<GetTeamRequest, IResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;
    private readonly IIdentity _identity;

    public GetTeamHandler(ITeamRepository teamRepository,
                          IMapper mapper,
                          IIdentity identity)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
        _identity = identity;
    }
    public async Task<IResult> Handle(GetTeamRequest request, CancellationToken cancellationToken)
    {
        Team teamDb = await _teamRepository.GetById(request.Id);

        if (teamDb is null)
        {
            return Response.NotFound(ErrorMessages.TeamNotFound, request.Id);
        }

        User teamLead = await _identity.SearchUserById(teamDb.TeamLeadId);

        TeamDetailsModel teamResponse = new()
        {
            Id = teamDb.Id,
            Name = teamDb.Name,
            TeamLead = _mapper.Map<UserModel>(teamLead)
        };

        foreach (var developerId in teamDb.DeveloperIds)
        {
            User currentDeveloper = await _identity.SearchUserById(developerId);
            teamResponse.Developers.Add(_mapper.Map<UserModel>(currentDeveloper));
        }

        return Response.OkData(teamResponse);
    }
}

