using AutoMapper;
using MediatR;

using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.Teams.Models;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Teams.Handlers;

public class GetAllTeamsHandler : IRequestHandler<GetAllTeamsRequest, IResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;
    private readonly IIdentity _identity;

    public GetAllTeamsHandler(ITeamRepository teamRepository,
                             IMapper mapper,
                             IIdentity identity)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
        _identity = identity;
    }

    public async Task<IResult> Handle(GetAllTeamsRequest request, CancellationToken cancellationToken)
    {
        var teamsDb = await _teamRepository.GetPaginatedTeams(request.PageSize ,request.NextPageKey);

        List<TeamDetailsModel> teamsResponse = new();

        foreach (Team team in teamsDb.Items)
        {
            User teamLead = await _identity.SearchUserById(team.TeamLeadId);

            TeamDetailsModel currentTeam = new()
            {
                Id = team.Id,
                Name = team.Name,
                TeamLead = _mapper.Map<UserModel>(teamLead)
            };

            foreach (var developerId in team.DeveloperIds)
            {
                User currentDeveloper = await _identity.SearchUserById(developerId);
                currentTeam.Developers.Add(_mapper.Map<UserModel>(currentDeveloper));
            }

            teamsResponse.Add(currentTeam);
        }

        return Response.OkPage(teamsResponse, request.PageSize, teamsDb.NextPageKey);
    }
}

