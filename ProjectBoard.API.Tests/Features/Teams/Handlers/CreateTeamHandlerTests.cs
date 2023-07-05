using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.Responses.Base.Enums;
using ProjectBoard.API.Features.Teams.Handlers;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Teams.Responses;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.Api.Tests.Features.Teams.Handlers;

public class CreateTeamHandlerTests
{
    [Fact]
    public async Task CreateTeamHandler_ValidRequest_ReturnsOkResultWithDataResponse()
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var teamLeadId = Guid.NewGuid().ToString();
        var teamId = Guid.NewGuid().ToString();
        var teamName = "Intern Team";

        var teamLead = new User(teamLeadId, "TestUser", "testuser@test.test");
        var savedTeam = new Team { Id = teamId, Name = teamName, TeamLeadId = teamLeadId };
        var createdTeamResponse = new TeamModel { Id = teamId, Name = teamName, TeamLeadId = teamLeadId };

        identityMock.Setup(mock => mock.SearchUserById(teamLeadId)).ReturnsAsync(teamLead);

        teamRepositoryMock.Setup(mock => mock.Save(It.IsAny<Team>())).ReturnsAsync(savedTeam);

        mapperMock.Setup(mock => mock.Map<TeamModel>(savedTeam)).Returns(createdTeamResponse);

        var resultFromHendler = Results.Ok(new DataResponse<TeamModel>(createdTeamResponse, ResponseStatus.Success()));

        var handler = new CreateTeamHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);

        var request = new CreateTeamRequest
        {
            Name = teamName,
            TeamLeadId = teamLeadId
        };

        // Act
        IResult result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equivalent(resultFromHendler, result);

        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<DataResponse<TeamModel>>>(result);

        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<DataResponse<TeamModel>>>(result);
        var dataResponse = okResult.Value;
        Assert.Equivalent(ResponseStatus.Success(), dataResponse.Status);
        Assert.Equivalent(createdTeamResponse, dataResponse.Payload);
        Assert.Equal(createdTeamResponse.Name, dataResponse.Payload.Name);
        Assert.Equal(createdTeamResponse.TeamLeadId, dataResponse.Payload.TeamLeadId);
        Assert.Equal(createdTeamResponse.Id, dataResponse.Payload.Id);
    }

    [Fact]
    public async Task CreateTeamHandler_TeamLeadNotFound_ReturnsNotFoundResultWithErrorMessage()
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var teamLeadId = Guid.NewGuid().ToString();
        var teamName = "Intern Team";

        // Set up the identity mock to return null for the team lead
        identityMock.Setup(mock => mock.SearchUserById(teamLeadId)).ReturnsAsync((User)null);

        var handler = new CreateTeamHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);

        var request = new CreateTeamRequest
        {
            Name = teamName,
            TeamLeadId = teamLeadId
        };

        // Act
        IResult result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<BaseResponse>>(result);
        var baseResponse = notFoundResult.Value;

        Assert.False(baseResponse.Status.IsSuccess);
        Assert.Contains(string.Format(ErrorMessages.TeamLeadNotFound, teamLeadId), baseResponse.Status.Message);
        Assert.Equal(Reason.InputError, baseResponse.Status.Reason);
    }

}

