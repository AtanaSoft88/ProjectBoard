using AutoMapper;
using Moq;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Data.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Handlers;
using ProjectBoard.API.Features.Projects.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectBoard.API.Tests.Features.Projects.Handlers.Data;

namespace ProjectBoard.API.Tests.Features.Projects.Handlers;
public class GetAllProjectsHandlerTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10, 10)]
    [InlineData(12, 2)]
    [InlineData(12, 10)]        
    public async Task GetAllProjectsHandler_ValidRequest_ReturnsPagedDataResponse(int pageSize, int projectsCount)
    {
        // Arrange
        var projectRepositoryMock = new Mock<IProjectRepository>();
        var mapperMock = new Mock<IMapper>();
        var identityMock = new Mock<IIdentity>();

        var userId = Guid.NewGuid().ToString();

        var user = new User(userId, "TestUser", "testuser@test.test");
        var handler = new GetAllProjectsHandler(projectRepositoryMock.Object, mapperMock.Object, identityMock.Object);
        var request = new GetAllProjectsRequest { PageSize = pageSize };
        
        var projects = DataModelInitializer.GetProjectsData(projectsCount);
        var response = new PaginationResult<Project>(projects, null);
        projectRepositoryMock.Setup(r => r.GetPaginatedProjects(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(response);

        identityMock.Setup(i => i.SearchUserById(It.IsAny<string>()))
            .ReturnsAsync(user);

        mapperMock.Setup(m => m.Map<UserModel>(It.IsAny<User>()))
            .Returns(new UserModel());

        // Act
        var result = await handler.Handle(request, new CancellationToken());

        // Assert
        var okResult = Assert.IsType<Ok<PagedDataResponse<ProjectDetailsModel>>>(result);
        var dataResponse = okResult.Value;

        Assert.NotNull(dataResponse);
        Assert.Equivalent(ResponseStatus.Success(), dataResponse.Status);
        Assert.Equal(projectsCount, dataResponse.Payload.Count());
    }
}
