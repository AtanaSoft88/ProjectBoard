using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using ProjectBoard.API.Features.Projects.Handlers;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using Moq;
using ProjectBoard.API.Http;
using ProjectBoard.Data.Abstractions.Enums;
using ProjectBoard.Identity.Abstractions.Models;
using ProjectBoard.Identity.Abstractions;
using AutoMapper;
using ProjectBoard.API.Features.Projects.Responses;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Tests.Features.Projects.Handlers.Data;
using ProjectBoard.API.Responses.Base;

namespace ProjectBoard.API.Tests.Features.Projects.Handlers;
public class UpdateProjectHandlerTests
{
    [Fact]
    public async Task UpdateProjectHandler_WhenProjectIsNotFound_ReturnsResultsNotFound()
    {
        // Arrange
        string projectId = Guid.NewGuid().ToString();
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
        .Setup(m => m.GetSingle(It.IsAny<string>()))
                            .ReturnsAsync(null as Project);
        var projectRequest = new UpdateProjectRequest()
        {
            Id = projectId
        };

        var handlerUnderTest = new UpdateProjectHandler(projectRepositoryMock.Object, null, null, null);

        // Act
        IResult result = await handlerUnderTest.Handle(projectRequest, new CancellationToken());

        //Assert
        projectRepositoryMock.Verify(x => x.GetSingle(It.IsAny<string>()), Times.Once);
        NotFound<BaseResponse> assertionResult = Assert.IsType<NotFound<BaseResponse>>(result);
        Assert.Equal(ErrorMessages.ProjectNotFoundById, assertionResult.Value?.Status.Message);
    }

    [Fact]
    public async Task UpdateProjectHandler_WhenNoCoincidenceBetweenCurrentLoggedUserAndDbUser_ReturnsResultsNotFound()
    {
        // Arrange
        string projectId = Guid.NewGuid().ToString();
        string projectManagerId = Guid.NewGuid().ToString();
        string currentUserId = Guid.NewGuid().ToString();

        var project = new Project()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };

        var userFromDb = new User(projectManagerId, "Pm-Name", "mail@.com");
        var currentLoggedUser = new CurrentUser() { UserId = currentUserId };

        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
        .Setup(m => m.GetSingle(It.IsAny<string>()))
                            .ReturnsAsync(project);
        var executionContextMock = new Mock<IExecutionContext>();
        executionContextMock.Setup(e => e.GetCurrentIdentity()).Returns(currentLoggedUser);
        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.SearchUserById(projectManagerId)).ReturnsAsync(userFromDb);
        var handlerUnderTest = new UpdateProjectHandler(projectRepositoryMock.Object, null, identityMock.Object, executionContextMock.Object);
        var request = new UpdateProjectRequest()
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
    public async Task UpdateProjectHandler_WhenProjectManagerIdNotFound_ReturnsResultsNotFound(string projectManagerId, string currentUserId)
    {
        // Arrange
        string projectId = Guid.NewGuid().ToString();
        var currentLoggedUser = new CurrentUser() { UserId = currentUserId };
        var project = new Project()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };

        var executionContextMock = new Mock<IExecutionContext>();
        executionContextMock.Setup(e => e.GetCurrentIdentity()).Returns(currentLoggedUser);
        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.SearchUserById(projectManagerId)).ReturnsAsync(null as User);
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
        .Setup(m => m.GetSingle(It.IsAny<string>()))
                            .ReturnsAsync(project);
        var handlerUnderTest = new UpdateProjectHandler(projectRepositoryMock.Object, null, identityMock.Object, executionContextMock.Object);
        var updateRequest = new UpdateProjectRequest()
        {
            Name = "Online-Tax-Calculator",
            Description = "To be able to calculate and collect results",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
        };

        // Act
        IResult result = await handlerUnderTest.Handle(updateRequest, new CancellationToken());

        //Assert
        NotFound<BaseResponse> assertionResult = Assert.IsType<NotFound<BaseResponse>>(result);
        Assert.True(assertionResult.Value?.Status?.Message?.Equals(ErrorMessages.PorjectManagerMismatch));
        Assert.Equal(StatusCodes.Status404NotFound, assertionResult.StatusCode);
    }

    [Theory]
    [InlineData(ProjectStatus.Approved, AssignmentStatus.Done)]
    [InlineData(ProjectStatus.Done, AssignmentStatus.Done)]
    [InlineData(ProjectStatus.InProgress, AssignmentStatus.Done)]
    [InlineData(ProjectStatus.NotStarted, AssignmentStatus.Done)]
    public async Task UpdateProjectHandler_WhenAllTasksAreDone_ReturnsResultsOk_AndProjectStatusCanBeAnyType(ProjectStatus projectStatus, AssignmentStatus taskStatus)
    {
        //Arrange
        string projectManagerId = "823fad29-02ed-4df4-b462-d38a168e060d";
        string currentUserId = "823fad29-02ed-4df4-b462-d38a168e060d";
        string projectId = Guid.NewGuid().ToString();
        List<Assignment> assignments = DataModelInitializer.GetAssignmentData(taskStatus);
        List<AssignmentModel> assignmentModels = DataModelInitializer.GetAssignmentModelData(taskStatus);

        var project = new Project()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = ProjectStatus.NotStarted,
            ProjectManagerId = projectManagerId,
            Assignments = assignments,
            TeamId = null,
        };
        var projectModel = new ProjectModel()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = projectStatus,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };
        var userFromDb = new User(projectManagerId, "Pm-Name", "mail@.com");
        var currentLoggedUser = new CurrentUser() { UserId = currentUserId };

        var executionContextMock = new Mock<IExecutionContext>();
        executionContextMock.Setup(e => e.GetCurrentIdentity()).Returns(currentLoggedUser);
        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.SearchUserById(projectManagerId)).ReturnsAsync(userFromDb);
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
        .Setup(m => m.GetSingle(It.IsAny<string>()))
                            .ReturnsAsync(project);
        projectRepositoryMock
        .Setup(m => m.Update(It.IsAny<Project>()))
                            .ReturnsAsync(project);

        var projectResponse = new ProjectModel()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            ProjectManagerId = projectManagerId,
            Status = projectStatus,
            TeamId = null
        };
        var updateRequest = new UpdateProjectRequest()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = projectStatus,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };
        var mapperMock = new Mock<IMapper>();
        mapperMock
                 .Setup(m => m.Map<ProjectModel>(project))
                 .Returns(projectModel);
        mapperMock
                 .Setup(m => m.Map<Project>(updateRequest))
                 .Returns(project);

        var handlerUnderTest = new UpdateProjectHandler(projectRepositoryMock.Object, mapperMock.Object, identityMock.Object, executionContextMock.Object);        
        IResult handlerExpectedResult = Response.OkData(projectResponse);

        //Act
        IResult result = await handlerUnderTest.Handle(updateRequest, new CancellationToken());

        //Assert
        projectRepositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.AtLeastOnce);
        Ok<DataResponse<ProjectModel>> assertionResult = Assert.IsType<Ok<DataResponse<ProjectModel>>>(result);        
        Assert.Equal(updateRequest.Status, assertionResult.Value?.Payload.Status);
        Assert.Equivalent(handlerExpectedResult, result, strict: true);
    }

    [Theory]
    [InlineData(ProjectStatus.Approved, AssignmentStatus.AwaitingProgress)]
    [InlineData(ProjectStatus.Done, AssignmentStatus.AwaitingProgress)]
    public async Task UpdateProjectHandler_WhenAllTasksAreNotDone_ReturnsResultsOk_AndProjectStatusCanNotBeDonеOrApproved(
                                                                                ProjectStatus projectStatus, AssignmentStatus taskStatus)
    {
        //Arrange
        string projectManagerId = "823fad29-02ed-4df4-b462-d38a168e060d";
        string currentUserId = "823fad29-02ed-4df4-b462-d38a168e060d";
        string projectId = Guid.NewGuid().ToString();
        List<Assignment> assignments = DataModelInitializer.GetAssignmentData(taskStatus);
        List<AssignmentModel> assignmentModels = DataModelInitializer.GetAssignmentModelData(taskStatus);

        var project = new Project()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = projectStatus,
            ProjectManagerId = projectManagerId,
            Assignments = assignments,
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
        var userFromDb = new User(projectManagerId, "Pm-Name", "mail@.com");
        var currentLoggedUser = new CurrentUser() { UserId = currentUserId };

        var executionContextMock = new Mock<IExecutionContext>();
        executionContextMock.Setup(e => e.GetCurrentIdentity()).Returns(currentLoggedUser);
        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.SearchUserById(projectManagerId)).ReturnsAsync(userFromDb);
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
        .Setup(m => m.GetSingle(It.IsAny<string>()))
                            .ReturnsAsync(project);
        projectRepositoryMock
        .Setup(m => m.Update(It.IsAny<Project>()))
                            .ReturnsAsync(project);
        var updateRequest = new UpdateProjectRequest()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = projectStatus,
            ProjectManagerId = projectManagerId,
            TeamId = null,
        };
        var mapperMock = new Mock<IMapper>();
        mapperMock
                 .Setup(m => m.Map<ProjectModel>(project))
                 .Returns(projectResponse);
        mapperMock
                 .Setup(m => m.Map<Project>(updateRequest))
                 .Returns(project);
        var handlerUnderTest = new UpdateProjectHandler(projectRepositoryMock.Object, mapperMock.Object, identityMock.Object, executionContextMock.Object);

        IResult handlerExpectedResult = Response.BadRequest(ErrorMessages.ProjectNotFinished);

        //Act
        IResult result = await handlerUnderTest.Handle(updateRequest, new CancellationToken());

        //Assert
        projectRepositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.Never);        
        Assert.Equal(ProjectStatus.InProgress, projectResponse.Status);
        Assert.NotEqual(ProjectStatus.Done, projectResponse.Status);
        Assert.NotEqual(ProjectStatus.Approved, projectResponse.Status);
        Assert.Equivalent(handlerExpectedResult, result);
    }
}
