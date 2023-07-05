using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.TeamMembers.Handlers;
using ProjectBoard.API.Features.TeamMembers.Requests;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Tests.Features.TeamMembers.Handlers;

public class CreateTeamMemberHandlerTests
{
    [Fact]
    public async Task Handle_NonExistingTeam_ReturnNotFoundResult()
    {
        //Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest()
        {
            TeamId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
        };
        Mock<IMapper> mapperMock = new Mock<IMapper>();
        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync((Team)null);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();

        CreateTeamMemberHandler sut = new CreateTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        //Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        //Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        Assert.Equivalent(Results.NotFound(new BaseResponse(ResponseStatus.Error(string.Format(
            ErrorMessages.TeamNotFound,
            request.TeamId)))), act);
    }

    [Fact]
    public async Task Handle_NonExistingUser_ReturnNotFoundResult()
    {
        //Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest()
        {
            TeamId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
        };

        Team team = new Team()
        {
            Id = request.TeamId,
        };

        Mock<IMapper> mapperMock = new Mock<IMapper>();
        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync(team);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();
        identityMock.Setup(identity => identity.SearchUserById(request.UserId)).ReturnsAsync((User)null);

        CreateTeamMemberHandler sut = new CreateTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        //Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        //Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        identityMock.Verify(mock => mock.SearchUserById(It.IsAny<string>()), Times.Once());
        Assert.NotNull(act);
        Assert.Equivalent(Results.NotFound(new BaseResponse(ResponseStatus.Error(string.Format(
            ErrorMessages.UserIdNotFound,
            request.UserId)))), act);
    }

    [Fact]
    public async Task Handle_ExistMemberWithThisIdInTeam_ReturnBadRequestResult()
    {
        //Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest()
        {
            TeamId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
        };
        User user = new User(request.UserId, "Test", "testing.testing@testing");
        Team team = new Team()
        {
            Id = request.TeamId,
            DeveloperIds = new List<string>()
            {
                request.UserId,
            }
        };

        Mock<IMapper> mapperMock = new Mock<IMapper>();
        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync(team);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();
        identityMock.Setup(identity => identity.SearchUserById(request.UserId)).ReturnsAsync(user);

        CreateTeamMemberHandler sut = new CreateTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        //Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        //Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        identityMock.Verify(mock => mock.SearchUserById(It.IsAny<string>()), Times.Once());
        Assert.NotNull(act);
        Assert.Contains(user.Id, team.DeveloperIds);
        Assert.Equivalent(Results.BadRequest(new BaseResponse(ResponseStatus.Error(ErrorMessages.ExistingMember))), act);
    }

    [Fact]
    public async Task Handle_OnTeamAndUserExistAddMember_ReturnsOkResultWithMembers()
    {
        // Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest
        {
            TeamId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
        };
        User user1 = new User(Guid.NewGuid().ToString(), "Test1", "test1@test1.com");
        User user2 = new User(Guid.NewGuid().ToString(), "Test2", "test2@test2.com");
        User userForAdd = new User(request.UserId, "Jhon", "jhonTest@jhonTest*1.com");
        Team team = new Team()
        {
            DeveloperIds = { user1.Id, user2.Id }
        };
        List<User> users = new List<User>() { user1, user2, userForAdd };
        List<UserModel> members = new List<UserModel>
        {
            new UserModel() {Id = user1.Id, Username = user1.Username, Email = user1.Email },
            new UserModel() {Id = user2.Id, Username = user1.Username, Email = user1.Email },
            new UserModel {Id = userForAdd.Id, Username = userForAdd.Username, Email = userForAdd.Email}
        };

        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync(team);
        teamRepositoryMock.Setup(teamRepository => teamRepository.Update(team)).ReturnsAsync(team);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();
        identityMock.Setup(identity => identity.SearchUserById(request.UserId)).ReturnsAsync(userForAdd);
        identityMock.Setup(identity => identity.SearchUserById(user1.Id)).ReturnsAsync(user1);
        identityMock.Setup(identity => identity.SearchUserById(user2.Id)).ReturnsAsync(user2);
        Mock<IMapper> mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<List<UserModel>>(users)).Returns(members);

        CreateTeamMemberHandler sut = new CreateTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        // Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        // Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        teamRepositoryMock.Verify(mock => mock.Update(It.IsAny<Team>()), Times.Once());
        identityMock.Verify(mock => mock.SearchUserById(It.IsAny<string>()), Times.Exactly(4));
        mapperMock.Verify(mock => mock.Map<List<UserModel>>(users));
        Assert.NotNull(act);
        Ok<DataResponse<List<UserModel>>> okResult = Assert.IsType<Ok<DataResponse<List<UserModel>>>>(act);
        Assert.IsType<Ok<DataResponse<List<UserModel>>>>(okResult);
        Assert.Contains(userForAdd.Id, team.DeveloperIds);
        Assert.Equal(members, okResult.Value.Payload);
    }
}