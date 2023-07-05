using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.Responses.Base.Enums;
using ProjectBoard.API.Features.Teams.Handlers;
using ProjectBoard.API.Features.Teams.Models;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Tests.Features.Teams.Handlers;

public class GetTeamHandlerTests
{
    [Fact]
    public async Task GetTeamHandler_ValidRequest_ReturnsOkResultWithDataResponse()
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var teamId = Guid.NewGuid().ToString();
        var teamName = "Intern Team";
        var teamLeadId = Guid.NewGuid().ToString();
        var developerIds = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

        var request = new GetTeamRequest { Id = teamId };

        var team = new Team { Id = teamId, Name = teamName, TeamLeadId = teamLeadId, DeveloperIds = developerIds };
        var teamLead = new User(teamLeadId, "TestUser", "testuser@test.test");
        var developers = new List<User> { new User(developerIds[0], "Developer1", "dev1@test.test"), new User(developerIds[1], "Developer2", "dev2@test.test") };

        var teamDetailedModel = new TeamDetailsModel
        {
            Id = teamId,
            Name = teamName,
            TeamLead = new UserModel
            {
                Id = teamLeadId,
                Username = "TestUser",
                Email = "testuser@test.test"
            },
            Developers = new List<UserModel>
            {
                new UserModel { Id = developerIds[0], Username = "Developer1", Email = "dev1@test.test" },
                new UserModel { Id = developerIds[1], Username = "Developer2", Email = "dev2@test.test" }
            }
        };

        teamRepositoryMock.Setup(mock => mock.GetById(teamId)).ReturnsAsync(team);
        identityMock.Setup(mock => mock.SearchUserById(teamLeadId)).ReturnsAsync(teamLead);
        identityMock.Setup(mock => mock.SearchUserById(developerIds[0])).ReturnsAsync(developers[0]);
        identityMock.Setup(mock => mock.SearchUserById(developerIds[1])).ReturnsAsync(developers[1]);
        mapperMock.Setup(mock => mock.Map<UserModel>(teamLead)).Returns(teamDetailedModel.TeamLead);
        mapperMock.Setup(mock => mock.Map<UserModel>(developers[0])).Returns(teamDetailedModel.Developers[0]);
        mapperMock.Setup(mock => mock.Map<UserModel>(developers[1])).Returns(teamDetailedModel.Developers[1]);

        var expectedResult = Results.Ok(new DataResponse<TeamDetailsModel>(teamDetailedModel, ResponseStatus.Success()));

        var handler = new GetTeamHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<DataResponse<TeamDetailsModel>>>(result);
        var dataResponse = okResult.Value;

        // Assert
        Assert.Equivalent(expectedResult, result);
        Assert.Equivalent(teamDetailedModel, dataResponse.Payload);
        Assert.Equal(teamDetailedModel.Name, dataResponse.Payload.Name);
        Assert.Equal(teamDetailedModel.TeamLead.Username, dataResponse.Payload.TeamLead.Username);
        Assert.Equal(teamDetailedModel.Developers[0].Username, dataResponse.Payload.Developers[0].Username);
    }

    [Fact]
    public async Task GetTeamHandler_InvalidTeamId_ReturnsNotFoundResult()
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var teamId = Guid.NewGuid().ToString();
        var request = new GetTeamRequest { Id = teamId };

        teamRepositoryMock.Setup(mock => mock.GetById(teamId)).ReturnsAsync((Team)null);

        var expectedResult = Results.NotFound(new BaseResponse(ResponseStatus.Error(string.Format(ErrorMessages.TeamNotFound, teamId))));

        var handler = new GetTeamHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<BaseResponse>>(result);
        var baseResponse = badRequestResult.Value;

        // Assert
        Assert.Equivalent(expectedResult, result);
        Assert.False(baseResponse.Status.IsSuccess);
        Assert.Contains(string.Format(ErrorMessages.TeamNotFound, teamId), baseResponse.Status.Message);
        Assert.Equal(Reason.InputError, baseResponse.Status.Reason);
    }
}

