using AutoMapper;
using Moq;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Projects.Handlers;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.Data.Abstractions.Enums;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using ProjectBoard.API.Features.Projects.Models;
using ProjectBoard.API.Features.Projects.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.API.Tests.Features.Projects.Handlers.Data;
using ProjectBoard.API.Responses.Base;

namespace ProjectBoard.API.Tests.Features.Projects.Handlers;
public class GetProjectHandlerTests
{
    [Fact]
    public async Task GetProjectHandler_WhenProjectIsNotFound_Returns_Results_Not_Found()
    {
        // Arrange
        string projectId = Guid.NewGuid().ToString();
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
        .Setup(m => m.GetSingle(It.IsAny<string>()))
                            .ReturnsAsync(null as Project);
        var projectRequest = new GetProjectRequest()
        {
            Id = projectId
        };

        var handlerUnderTest = new GetProjectHandler(projectRepositoryMock.Object, null, null);

        // Act
        IResult result = await handlerUnderTest.Handle(projectRequest, new CancellationToken());

        //Assert
        projectRepositoryMock.Verify(x => x.GetSingle(It.IsAny<string>()), Times.Once);
        NotFound<BaseResponse> assertionResult = Assert.IsType<NotFound<BaseResponse>>(result);
        Assert.Equal(ErrorMessages.ProjectNotFoundById, assertionResult.Value?.Status.Message);
    }

    [Fact]
    public async Task GetProjectHandler_WhenProjectFoundSuccessfully_ReturnsResultsOk()
    {
        // Arrange
        AssignmentStatus status = AssignmentStatus.InProgress;
        string projectId = Guid.NewGuid().ToString();
        string projectManagerId = Guid.NewGuid().ToString();
        List<Assignment> assignments = DataModelInitializer.GetAssignmentData(status);
        List<AssignmentModel> assignmentModels = DataModelInitializer.GetAssignmentModelData(status);

        var project = new Project()
        {
            Id = projectId,
            Name = "TestProjectModel",
            Description = "Test",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = projectManagerId,
            Assignments = assignments,
            TeamId = null,
        };
        var dbUser = new User(projectManagerId, "Pm-Name", "mail@.com");
        var projectManager = new UserModel() { Id = projectManagerId, Email = "mail@test.com", Username = "UserName-1" };
        var identityMock = new Mock<IIdentity>();
        identityMock
                    .Setup(i => i.SearchUserById(projectManagerId))
                    .ReturnsAsync(dbUser);
        var mapperMock = new Mock<IMapper>();
        mapperMock
                 .Setup(m => m.Map<UserModel>(dbUser))
                 .Returns(projectManager);
        mapperMock
                 .Setup(m => m.Map<List<AssignmentModel>>(project.Assignments))
                 .Returns(assignmentModels);
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock
        .Setup(m => m.GetSingle(It.IsAny<string>()))
                            .ReturnsAsync(project);

        var projectResponse = new ProjectDetailsModel()
        {
            Id = project.Id,
            Status = project.Status,
            Assignments = assignmentModels,
            Description = project.Description,
            Name = project.Name,
            ProjectManager = projectManager,
            TeamId = null
        };
        IResult handlerExpectedResult = Response.OkData(projectResponse);
        var projectRequest = new GetProjectRequest()
        {
            Id = projectId
        };

        var handlerUnderTest = new GetProjectHandler(projectRepositoryMock.Object, mapperMock.Object, identityMock.Object);

        // Act
        IResult result = await handlerUnderTest.Handle(projectRequest, new CancellationToken());

        //Assert
        projectRepositoryMock.Verify(x => x.GetSingle(It.IsAny<string>()), Times.AtLeastOnce);
        Assert.Equivalent(handlerExpectedResult, result, strict: true);
        Ok<DataResponse<ProjectDetailsModel>> assertionResult = Assert.IsType<Ok<DataResponse<ProjectDetailsModel>>>(result);
        Assert.Equivalent(projectResponse, assertionResult.Value?.Payload);
    }
}
