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

namespace ProjectBoard.API.Tests.Features.Teams.Handlers;

public class UpdateTeamHandlerTests
{
    [Fact]
    public async Task UpdateTeamHandler_ValidRequest_ReturnsOkResultWithDataResponse()
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var teamId = Guid.NewGuid().ToString();
        var teamName = "Intern Team";
        var teamLeadId = Guid.NewGuid().ToString();

        var request = new UpdateTeamRequest
        {
            Id = teamId,
            Name = teamName,
            TeamLeadId = teamLeadId
        };

        var team = new Team { Id = teamId, Name = teamName, TeamLeadId = teamLeadId };
        var updatedTeam = new Team { Id = teamId, Name = "Updated Team", TeamLeadId = teamLeadId };
        var teamModel = new TeamModel { Id = teamId, Name = teamName, TeamLeadId = teamLeadId };
        var updatedTeamModel = new TeamModel { Id = teamId, Name = "Updated Team", TeamLeadId = teamLeadId };

        teamRepositoryMock.Setup(mock => mock.GetById(teamId)).ReturnsAsync(team);
        identityMock.Setup(mock => mock.SearchUserById(teamLeadId)).ReturnsAsync(new User(teamLeadId, "TestUser", "testuser@test.test"));
        teamRepositoryMock.Setup(mock => mock.Save(team)).ReturnsAsync(updatedTeam);
        mapperMock.Setup(mock => mock.Map<TeamModel>(updatedTeam)).Returns(updatedTeamModel);

        var expectedResult = Results.Ok(new DataResponse<TeamModel>(updatedTeamModel, ResponseStatus.Success()));

        var handler = new UpdateTeamHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<DataResponse<TeamModel>>>(result);
        var dataResponse = okResult.Value;
        // Assert
        Assert.Equivalent(expectedResult, result);

        Assert.Equivalent(ResponseStatus.Success(), dataResponse.Status);
        Assert.Equivalent(updatedTeamModel, dataResponse.Payload);
        Assert.Equal(updatedTeamModel.Name, dataResponse.Payload.Name);
        Assert.Equal(updatedTeamModel.TeamLeadId, dataResponse.Payload.TeamLeadId);
        Assert.Equal(updatedTeamModel.Id, dataResponse.Payload.Id);
    }

    [Fact]
    public async Task UpdateTeamHandler_InvalidTeamId_ReturnsNotFoundResult()
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var teamId = Guid.NewGuid().ToString();
        var request = new UpdateTeamRequest { Id = teamId };

        teamRepositoryMock.Setup(mock => mock.GetById(teamId)).ReturnsAsync((Team)null);

        var expectedResult = Results.NotFound(new BaseResponse(ResponseStatus.Error(string.Format(ErrorMessages.TeamNotFound, teamId))));
        var handler = new UpdateTeamHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<BaseResponse>>(result);
        var baseResponse = notFoundResult.Value;

        // Assert
        Assert.Equivalent(expectedResult, result);
        Assert.False(baseResponse.Status.IsSuccess);
        Assert.Contains(string.Format(ErrorMessages.TeamNotFound, teamId), baseResponse.Status.Message);
        Assert.Equal(Reason.InputError, baseResponse.Status.Reason);
    }

    [Fact]
    public async Task UpdateTeamHandler_InvalidTeamLeadId_ReturnsNotFoundResult()
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var teamId = Guid.NewGuid().ToString();
        var teamLeadId = Guid.NewGuid().ToString();
        var request = new UpdateTeamRequest { Id = teamId, TeamLeadId = teamLeadId };

        teamRepositoryMock.Setup(mock => mock.GetById(teamId)).ReturnsAsync(new Team());
        identityMock.Setup(mock => mock.SearchUserById(teamLeadId)).ReturnsAsync((User)null);

        var expectedResult = Results.NotFound(new BaseResponse(ResponseStatus.Error(string.Format(ErrorMessages.TeamLeadNotFound, request.TeamLeadId))));
        var handler = new UpdateTeamHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<BaseResponse>>(result);
        var baseResponse = notFoundResult.Value;

        // Assert
        Assert.Equivalent(expectedResult, result);
        Assert.False(baseResponse.Status.IsSuccess);
        Assert.Contains(string.Format(ErrorMessages.TeamLeadNotFound, request.TeamLeadId), baseResponse.Status.Message);
        Assert.Equal(Reason.InputError, baseResponse.Status.Reason);
    }
}

