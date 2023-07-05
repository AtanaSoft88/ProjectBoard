using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProjectBoard.API.Features.Assignments.Handlers;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Projects.Models;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Enums;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Tests.Features.Assignments.Handlers;

public class GetAllAssignmentsHandlerTests
{
    [Fact]
    public async Task GetAllAssignmentsHandler_ValidData_ReturnsOkResultWithDataResponse()
    {
        // Arrange
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string userId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6as";

        GetAllAssignmentsRequest request = new()
        {
            ProjectId = projectId,
        };

        User user = new(userId, "Random Username", "random@email.com");
        UserModel userModel = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        List<Assignment> assignments = new()
        {
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf28973532a",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            },
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735asd",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            },
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735ghj",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            }
        };

        Project project = new()
        {
            Id = projectId,
            Name = "test",
            Assignments = assignments
        };

        ProjectInfoModel projectDetails = new()
        {
            Id = project.Id,
            Name = project.Name,
        };

        IEnumerable<AssignmentDetailsModel> expectedAssignments = new List<AssignmentDetailsModel>()
        {
            new AssignmentDetailsModel
            {
                Id = "8a896114-102d-45f1-8a8e-acf28973532a",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                Developer = userModel,
                ProjectDetails = projectDetails
            },
            new AssignmentDetailsModel
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735asd",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                Developer = userModel,
                ProjectDetails = projectDetails
            },
            new AssignmentDetailsModel
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735ghj",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                Developer = userModel,
                ProjectDetails = projectDetails
            }
        };

        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));

        Mock<IIdentity> identityContextMock = new();
        identityContextMock
            .Setup(t => t.SearchUserById(It.IsAny<string>()))
            .Returns(Task.FromResult(user));

        GetAllAssignmentsHandler sut = new(projectRepositoryMock.Object, identityContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, CancellationToken.None);

        // Assert

        Ok<PagedDataResponse<AssignmentDetailsModel>> okResult = Assert.IsType<Ok<PagedDataResponse<AssignmentDetailsModel>>>(result);
        PagedDataResponse<AssignmentDetailsModel> resultData = okResult.Value;
        IEnumerable<AssignmentDetailsModel> assignmentsResponsePayload = resultData.Payload;
        Assert.Equal(expectedAssignments.Count(), assignmentsResponsePayload.Count());
    }

    [Fact]
    public async Task GetAllAssignmentsHandler_WithUnexistingProjectId_ReturnsBadRequestResultWithBaseResponse()
    {
        // Arrange
        string invalidProjectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6in";

        GetAllAssignmentsRequest request = new()
        {
            ProjectId = invalidProjectId,
        };

        ResponseStatus expectedResponseStatus = new()
        {
            IsSuccess = false,
            Message = string.Format(ErrorMessages.ProjectNotFound, request.ProjectId)
        };

        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))!
            .ReturnsAsync((Project)null);
        Mock<ITeamRepository> teamRepositoryMock = new();

        Mock<IIdentity> identityContextMock = new();

        GetAllAssignmentsHandler sut = new(projectRepositoryMock.Object, identityContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        NotFound<BaseResponse> badRequestResult = Assert.IsType<NotFound<BaseResponse>>(result);
        BaseResponse resultData = badRequestResult.Value!;
        ResponseStatus assignmentResponseStatus = resultData.Status;
        Assert.Equivalent(expectedResponseStatus, assignmentResponseStatus, strict: true);
    }

    [Fact]
    public async Task GetAllAssignmentsHandler_ValidPageSize_ReturnsOkResultWithPageDataResponse()
    {
        // Arrange
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string userId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6as";

        GetAllAssignmentsRequest request = new()
        {
            ProjectId = projectId,
            PageSize = 1,
        };

        User user = new(userId, "Random Username", "random@email.com");
        UserModel userModel = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        List<Assignment> assignments = new()
        {
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf28973532a",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            },
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735asd",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            },
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735ghj",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            }
        };

        Project project = new()
        {
            Id = projectId,
            Name = "test",
            Assignments = assignments
        };

        ProjectInfoModel projectDetails = new()
        {
            Id = project.Id,
            Name = project.Name,
        };

        IEnumerable<AssignmentDetailsModel> expectedAssignments = new List<AssignmentDetailsModel>()
        {
            new AssignmentDetailsModel
            {
                Id = "8a896114-102d-45f1-8a8e-acf28973532a",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                Developer = userModel,
                ProjectDetails = projectDetails
            }
        };

        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));

        Mock<IIdentity> identityContextMock = new();
        identityContextMock
            .Setup(t => t.SearchUserById(It.IsAny<string>()))
            .Returns(Task.FromResult(user));

        GetAllAssignmentsHandler sut = new(projectRepositoryMock.Object, identityContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, CancellationToken.None);

        // Assert

        Ok<PagedDataResponse<AssignmentDetailsModel>> okResult = Assert.IsType<Ok<PagedDataResponse<AssignmentDetailsModel>>>(result);
        PagedDataResponse<AssignmentDetailsModel> resultData = okResult.Value;
        IEnumerable<AssignmentDetailsModel> assignmentsResponsePayload = resultData.Payload;
        Assert.Equal(expectedAssignments.Count(), assignmentsResponsePayload.Count());
    }

    [Fact]
    public async Task GetAllAssignmentsHandler_NextPageKeyIsBiggerThanActualPagesCount_ReturnsOkResultPageDataResponseWithEmptyArray()
    {
        // Arrange
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string userId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6as";

        GetAllAssignmentsRequest request = new()
        {
            ProjectId = projectId,
            PageSize = 1,
            NextPageKey = "10",
        };

        User user = new(userId, "Random Username", "random@email.com");
        UserModel userModel = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        List<Assignment> assignments = new()
        {
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf28973532a",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            },
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735asd",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            },
            new Assignment
            {
                Id = "8a896114-102d-45f1-8a8e-acf289735ghj",
                Name = "test",
                Description = "test description",
                Status = AssignmentStatus.AwaitingProgress,
                DeveloperId = userId
            }
        };

        Project project = new()
        {
            Id = projectId,
            Name = "test",
            Assignments = assignments
        };

        ProjectInfoModel projectDetails = new()
        {
            Id = project.Id,
            Name = project.Name,
        };

        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));

        Mock<IIdentity> identityContextMock = new();
        identityContextMock
            .Setup(t => t.SearchUserById(It.IsAny<string>()))
            .Returns(Task.FromResult(user));

        GetAllAssignmentsHandler sut = new(projectRepositoryMock.Object, identityContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, CancellationToken.None);

        // Assert

        Ok<PagedDataResponse<AssignmentDetailsModel>> okResult = Assert.IsType<Ok<PagedDataResponse<AssignmentDetailsModel>>>(result);
        PagedDataResponse<AssignmentDetailsModel> resultData = okResult.Value;
        IEnumerable<AssignmentDetailsModel> assignmentsResponsePayload = resultData.Payload;
        Assert.Empty(assignmentsResponsePayload);
        Assert.Null(resultData.PageInfo.NextPageKey);
    }
}
