using AutoMapper;
using Moq;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.Teams.Handlers;
using ProjectBoard.API.Features.Teams.Models;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.Data.Abstractions;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Tests.Features.Teams.Handlers;

public class GetAllTeamsHandlerTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10, 2)]
    public async Task GetAllTeamsHandler_ValidRequest_ReturnsPagedDataResponse(int pageSize, int teamCount)
    {
        // Arrange
        var teamRepositoryMock = new Mock<ITeamRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var userId = Guid.NewGuid().ToString();

        var user = new User(userId, "TestUser", "testuser@test.test");
        var handler = new GetAllTeamsHandler(teamRepositoryMock.Object, mapperMock.Object, identityMock.Object);
        var request = new GetAllTeamsRequest { PageSize = pageSize };

        var teams = Enumerable.Range(1, teamCount).Select(i => new Team
        {
            Id = i.ToString(),
            Name = $"Team {i}",
            TeamLeadId = userId
        }).ToList();

        var response = new PaginationResult<Team>(teams, null);
        teamRepositoryMock.Setup(r => r.GetPaginatedTeams(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(response);

        identityMock.Setup(i => i.SearchUserById(It.IsAny<string>()))
            .ReturnsAsync(user);

        mapperMock.Setup(m => m.Map<UserModel>(It.IsAny<User>()))
            .Returns(new UserModel());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<PagedDataResponse<TeamDetailsModel>>>(result);
        var dataResponse = okResult.Value;

        Assert.NotNull(dataResponse);
        Assert.Equivalent(ResponseStatus.Success(), dataResponse.Status);
        Assert.Equal(teamCount, dataResponse.Payload.Count());
    }
}

