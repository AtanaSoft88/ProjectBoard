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

public class RemoveTeamMemberHandlerTests
{
    [Fact]
    public async Task Handle_NonExistingTeam_ReturnNotFoundResult()
    {
        //Arrange
        RemoveTeamMemberRequest request = new RemoveTeamMemberRequest()
        {
            TeamId = Guid.NewGuid().ToString(),
            Id = Guid.NewGuid().ToString(),
        };
        Mock<IMapper> mapperMock = new Mock<IMapper>();
        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync((Team)null);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();

        RemoveTeamMemberHandler sut = new RemoveTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        //Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        //Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        Assert.NotNull(act);
        Assert.Equivalent(Results.NotFound(new BaseResponse(ResponseStatus.Error(string.Format(
            ErrorMessages.TeamNotFound,
            request.TeamId)))), act);
    }

    [Fact]
    public async Task Handle_NonExistingUser_ReturnNotFoundResult()
    {
        //Arrange
        RemoveTeamMemberRequest request = new RemoveTeamMemberRequest()
        {
            TeamId = Guid.NewGuid().ToString(),
            Id = Guid.NewGuid().ToString(),
        };

        Team team = new Team()
        {
            Id = request.TeamId,
        };

        Mock<IMapper> mapperMock = new Mock<IMapper>();
        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync(team);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();
        identityMock.Setup(identity => identity.SearchUserById(request.Id)).ReturnsAsync((User)null);

        RemoveTeamMemberHandler sut = new RemoveTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        //Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        //Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        identityMock.Verify(mock => mock.SearchUserById(It.IsAny<string>()), Times.Once());
        Assert.NotNull(act);
        Assert.Equivalent(Results.NotFound(new BaseResponse(ResponseStatus.Error(string.Format(
            ErrorMessages.UserIdNotFound,
            request.Id)))), act);
    }

    [Fact]
    public async Task Handle_NonExistMemberWithThisIdInTeam_ReturnNotFoundResult()
    {
        //Arrange
        RemoveTeamMemberRequest request = new RemoveTeamMemberRequest()
        {
            TeamId = Guid.NewGuid().ToString(),
            Id = Guid.NewGuid().ToString(),
        };
        User user = new User(request.Id, "Test", "testing.testing@testing");
        Team team = new Team()
        {
            Id = request.TeamId,
            DeveloperIds = new List<string>()
        };


        Mock<IMapper> mapperMock = new Mock<IMapper>();
        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync(team);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();
        identityMock.Setup(identity => identity.SearchUserById(request.Id)).ReturnsAsync(user);

        RemoveTeamMemberHandler sut = new RemoveTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        //Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        //Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        identityMock.Verify(mock => mock.SearchUserById(It.IsAny<string>()), Times.Once());
        Assert.NotNull(act);
        Assert.DoesNotContain(user.Id, team.DeveloperIds);
        Assert.Equivalent(Results.NotFound(new BaseResponse(ResponseStatus.Error(ErrorMessages.NotExistingMember))), act);
    }

    [Fact]
    public async Task Handle_OnTeamAndUserExistRemoveMember_ReturnOkResultWithMembers()
    {
        //Arrange
        RemoveTeamMemberRequest request = new RemoveTeamMemberRequest()
        {
            TeamId = Guid.NewGuid().ToString(),
            Id = Guid.NewGuid().ToString(),
        };
        User user1 = new User(Guid.NewGuid().ToString(), "Test1", "test1@test1.com");
        User user2 = new User(Guid.NewGuid().ToString(), "Test2", "test2@test2.com");
        User userForDelte = new User(request.Id, "Jhon", "jhonTest@jhonTest*1.com");
        Team team = new Team()
        {
            Id = request.TeamId,
            DeveloperIds = { user1.Id, user2.Id, userForDelte.Id }
        };
        List<User> users = new List<User>() { user1, user2 };
        List<UserModel> members = new List<UserModel>()
        {
            new UserModel() {Id = user1.Id, Username = user1.Username, Email = user1.Email },
            new UserModel() {Id = user2.Id, Username = user2.Username, Email = user2.Email },
        };

        Mock<IMapper> mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<List<UserModel>>(users)).Returns(members);
        Mock<ITeamRepository> teamRepositoryMock = new Mock<ITeamRepository>();
        teamRepositoryMock.Setup(teamRepository => teamRepository.GetById(request.TeamId)).ReturnsAsync(team);
        teamRepositoryMock.Setup(teamRepository => teamRepository.Update(team)).ReturnsAsync(team);
        Mock<IIdentity> identityMock = new Mock<IIdentity>();
        identityMock.Setup(identity => identity.SearchUserById(request.Id)).ReturnsAsync(userForDelte);
        identityMock.Setup(identity => identity.SearchUserById(user1.Id)).ReturnsAsync(user1);
        identityMock.Setup(identity => identity.SearchUserById(user2.Id)).ReturnsAsync(user2);

        RemoveTeamMemberHandler sut = new RemoveTeamMemberHandler(mapperMock.Object, teamRepositoryMock.Object, identityMock.Object);

        //Act
        IResult act = await sut.Handle(request, CancellationToken.None);

        //Assert
        teamRepositoryMock.Verify(mock => mock.GetById(It.IsAny<string>()), Times.Once());
        teamRepositoryMock.Verify(mock => mock.Update(It.IsAny<Team>()), Times.Once());
        identityMock.Verify(mock => mock.SearchUserById(It.IsAny<string>()), Times.Exactly(3));
        mapperMock.Verify(mock => mock.Map<List<UserModel>>(users));
        Assert.NotNull(act);
        Ok<DataResponse<List<UserModel>>> okResult = Assert.IsType<Ok<DataResponse<List<UserModel>>>>(act);
        Assert.IsType<Ok<DataResponse<List<UserModel>>>>(okResult);
        Assert.DoesNotContain(userForDelte.Id, team.DeveloperIds);
        Assert.Equivalent(members, okResult.Value.Payload);
    }
}
