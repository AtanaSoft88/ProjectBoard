using AutoMapper;
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

public class GetAssignmentHandlerTests
{
    [Fact]
    public async Task GetAssignmentHandler_ValidData_ReturnsOkResultWithDataResponse()
    {
        // Arrange
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string userId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6as";

        GetAssignmentRequest request = new()
        {
            ProjectId = projectId,
            Id = assignmentId,
        };

        User user = new(userId, "Random Username", "random@email.com");
        UserModel userModel = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        Assignment assignment = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
            DeveloperId = userId
        };

        Project project = new()
        {
            Id = projectId,
            Name = "test",
            Assignments = new List<Assignment> { assignment }
        };

        AssignmentDetailsModel assignmentResponseModel = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
            ProjectDetails = new ProjectInfoModel()
            {
                Id = project.Id,
                Name = project.Name,
            },
            Developer = userModel
        };

        Mock<IMapper> mapperMock = new();
        mapperMock
            .Setup(m => m.Map<Assignment>(request))
            .Returns(assignment);
        mapperMock
            .Setup(m => m.Map<AssignmentDetailsModel>(assignment))
            .Returns(assignmentResponseModel);
        mapperMock
            .Setup(m => m.Map<UserModel>(user))
            .Returns(userModel);

        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));
        projectRepositoryMock
           .Setup(p => p.Update(project))
           .Returns(Task.FromResult(project));

        Mock<IIdentity> identityContextMock = new();
        identityContextMock
            .Setup(t => t.SearchUserById(It.IsAny<string>()))
            .Returns(Task.FromResult(user));

        GetAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, identityContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        Ok<DataResponse<AssignmentDetailsModel>> okResult = Assert.IsType<Ok<DataResponse<AssignmentDetailsModel>>>(result);
        DataResponse<AssignmentDetailsModel> resultData = okResult.Value;
        AssignmentDetailsModel assignmentResponsePayload = resultData.Payload;
        Assert.Equivalent(assignmentResponseModel, assignmentResponsePayload, strict: true);
    }

    [Fact]
    public async Task GetAssignmentHandler_WithUnexistingProjectId_ReturnsBadRequestResultWithBaseResponse()
    {
        // Arrange
        string invalidProjectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6in";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";
        string userId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6as";

        User user = new(userId, "Random Username", "random@email.com");
        UserModel userModel = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        GetAssignmentRequest request = new()
        {
            Id = assignmentId,
            ProjectId = invalidProjectId,
        };

        Assignment assignment = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
        };

        ResponseStatus expectedResponseStatus = new()
        {
            IsSuccess = false,
            Message = string.Format(ErrorMessages.ProjectNotFound, request.ProjectId)
        };

        Mock<IMapper> mapperMock = new();
        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))!
            .ReturnsAsync((Project)null);
        Mock<ITeamRepository> teamRepositoryMock = new();

        Mock<IIdentity> identityContextMock = new();
        identityContextMock
            .Setup(t => t.SearchUserById(It.IsAny<string>()))
            .Returns(Task.FromResult(user));

        GetAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, identityContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        NotFound<BaseResponse> BadRequestResult = Assert.IsType<NotFound<BaseResponse>>(result);
        BaseResponse resultData = BadRequestResult.Value;
        ResponseStatus assignmentResponseStatus = resultData.Status;
        Assert.Equivalent(expectedResponseStatus, assignmentResponseStatus, strict: true);
    }

    [Fact]
    public async Task GetAssignmentHandler_WithUnexistingAssignmentId_ReturnsBadRequestResultWithBaseResponse()
    {
        // Arrange
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string invalidProjectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6in";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";
        string userId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6as";

        User user = new(userId, "Random Username", "random@email.com");
        UserModel userModel = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        GetAssignmentRequest request = new()
        {
            Id = assignmentId,
            ProjectId = invalidProjectId,
        };

        Project project = new()
        {
            Id = projectId,
            Name = "test",
        };

        Assignment assignment = new()
        {
            Id = "Unexist",
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
        };

        ResponseStatus expectedResponseStatus = new()
        {
            IsSuccess = false,
            Message = string.Format(ErrorMessages.AssignmentNotFound, request.Id)
        };

        Mock<IMapper> mapperMock = new();
        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))!
            .ReturnsAsync(project);
        Mock<ITeamRepository> teamRepositoryMock = new();

        Mock<IIdentity> identityContextMock = new();
        identityContextMock
            .Setup(t => t.SearchUserById(It.IsAny<string>()))
            .Returns(Task.FromResult(user));

        GetAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, identityContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        NotFound<BaseResponse> BadRequestResult = Assert.IsType<NotFound<BaseResponse>>(result);
        BaseResponse resultData = BadRequestResult.Value;
        ResponseStatus assignmentResponseStatus = resultData.Status;
        Assert.Equivalent(expectedResponseStatus, assignmentResponseStatus, strict: true);
    }
}
