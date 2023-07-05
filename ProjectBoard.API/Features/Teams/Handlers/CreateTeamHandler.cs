using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Teams.Responses;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Teams.Handlers;

public class CreateTeamHandler : IRequestHandler<CreateTeamRequest, IResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;
    private readonly IIdentity _identity;

    public CreateTeamHandler(ITeamRepository teamRepository,
                             IMapper mapper,
                             IIdentity identity)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
        _identity = identity;
    }

    public async Task<IResult> Handle(CreateTeamRequest request, CancellationToken cancellationToken)
    {
        User teamLead = await _identity.SearchUserById(request.TeamLeadId);

        if (teamLead is null)
        {
            return Response.NotFound(ErrorMessages.TeamLeadNotFound, request.TeamLeadId);
        }

        Team team = new()
        {
            Name = request.Name,
            TeamLeadId = request.TeamLeadId
        };

        Team teamsDb = await _teamRepository.Save(team);

        TeamModel teamResponse = _mapper.Map<TeamModel>(teamsDb);

        return Response.OkData(teamResponse);
    }
}


