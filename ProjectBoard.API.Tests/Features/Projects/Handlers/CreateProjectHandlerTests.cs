using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProjectBoard.API.Features.Projects.Handlers;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Responses;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Http;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Enums;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Tests.Features.Projects.Handlers;
public class CreateProjectHandlerTests
{
    [Fact]
    public async Task CreateProjectHandler_WhenNoCoincidenceBetweenCurrentLoggedUserAndDbUser_ReturnsResultsNotFound()
    {
        // Arrange
        string projectManagerId = Guid.NewGuid().ToString();
        string currentUserId = Guid.NewGuid().ToString();
        var identityMock = new Mock<IIdentity>();
        var executionContextMock = new Mock<IExecutionContext>();

        var userFromDb = new User(projectManagerId, "Pm-Name", "mail@.com");
        var currentLoggedUser = new CurrentUser() { UserId = currentUserId };

        executionContextMock.Setup(e => e.GetCurrentIdentity()).Returns(currentLoggedUser);
        identityMock.Setup(i => i.SearchUserById(projectManagerId)).ReturnsAsync(userFromDb);
        var handlerUnderTest = new CreateProjectHandler(null, null, executionContextMock.Object, identityMock.Object);
        var request = new CreateProjectRequest()
        {
            Name = "Online-Tax-Calculator",
            Description = "To be able to calculate and collect results",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
        };

        // Act
        IResult result = await handlerUnderTest.Handle(request, new CancellationToken());

        //Assert
        NotFound<BaseResponse> assertionResult = Assert.IsType<NotFound<BaseResponse>>(result);
        Assert.True(assertionResult.Value?.Status?.Message?.Equals(ErrorMessages.PorjectManagerMismatch));
        Assert.Equal(StatusCodes.Status404NotFound, assertionResult.StatusCode);
    }

    [Theory]
    [InlineData("823fad29-02ed-4df4-b462-d38a168e060d", "823fad29-02ed-4df4-b462-d38a168e060d")]
    [InlineData("823fad29-02ed-4df4-b462-d38a168e060d", "160139aa-550a-421a-a2f8-5b5cfab4d181")]
    public async Task CreateProjectHandler_WhenProjectManagerIdNotFound_ReturnsResultsNotFound(string projectManagerId, string currentUserId) 
    {
        // Arrange
        var identityMock = new Mock<IIdentity>();
        var executionContextMock = new Mock<IExecutionContext>();

        var currentLoggedUser = new CurrentUser() { UserId = currentUserId };
        executionContextMock.Setup(e => e.GetCurrentIdentity()).Returns(currentLoggedUser);
        identityMock.Setup(i => i.SearchUserById(projectManagerId)).ReturnsAsync(null as User);
        var handlerUnderTest = new CreateProjectHandler(null, null, executionContextMock.Object, identityMock.Object);
        var request = new CreateProjectRequest()
        {
            Name = "Online-Tax-Calculator",
            Description = "To be able to calculate and collect results",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
        };

        // Act
        IResult result = await handlerUnderTest.Handle(request, new CancellationToken());

        //Assert
        NotFound<BaseResponse> assertionResult = Assert.IsType<NotFound<BaseResponse>>(result);
        Assert.True(assertionResult.Value?.Status?.Message?.Equals(ErrorMessages.PorjectManagerMismatch));
        Assert.Equal(StatusCodes.Status404NotFound, assertionResult.StatusCode);
    }

    [Fact]
    public async Task CreateProjectHandler_OnSuccessfulSaveIntoDatabase_ReturnsStatusCode200()
    {
        // Arrange
        string projectId = Guid.NewGuid().ToString();
        string projectManagerId = Guid.NewGuid().ToString();            
        var currentLoggedUser = new CurrentUser() { UserId = projectManagerId };
        var dbUser = new User(projectManagerId, "Pm-Name", "mail@.com");        
        
        var project = new Project()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };
        var projectResponse = new ProjectModel()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };
        var mapperMock = new Mock<IMapper>();
        mapperMock
                 .Setup(m => m.Map<ProjectModel>(project))
                 .Returns(projectResponse);

        var identityMock = new Mock<IIdentity>();
        identityMock
                    .Setup(i => i.SearchUserById(projectManagerId))
                    .ReturnsAsync(dbUser);
        var executionContextMock = new Mock<IExecutionContext>();
        executionContextMock
                           .Setup(t => t.GetCurrentIdentity())
                           .Returns(currentLoggedUser);
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
                            .Setup(m => m.Create(It.IsAny<Project>()))
                            .ReturnsAsync(project);

        IResult handlerExpectedResult = Response.OkData(projectResponse);

        var projectRequest = new CreateProjectRequest()
        {               
            Name = "TestProjectModel",
            Description = "Test",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };
        
        var handlerUnderTest = new CreateProjectHandler(projectRepositoryMock.Object, mapperMock.Object, executionContextMock.Object, identityMock.Object);

        //Act
        IResult result = await handlerUnderTest.Handle(projectRequest, new CancellationToken());

        //Assert
        projectRepositoryMock.Verify(x => x.Create(It.IsAny<Project>()), Times.Once);
        Assert.Equivalent(handlerExpectedResult, result, strict:true);
        Ok<DataResponse<ProjectModel>> assertionResult = Assert.IsType<Ok<DataResponse<ProjectModel>>>(result);
        Assert.Equal(StatusCodes.Status200OK, assertionResult.StatusCode);
        Assert.Equivalent(projectResponse, assertionResult.Value?.Payload);            
    }
}
