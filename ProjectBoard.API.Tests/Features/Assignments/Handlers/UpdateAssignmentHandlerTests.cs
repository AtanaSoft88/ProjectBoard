using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProjectBoard.API.Features.Assignments.Handlers;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Http;
using ProjectBoard.API.Utilities;
using ProjectBoard.Data.Abstractions.Enums;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;

namespace ProjectBoard.API.Tests.Features.Assignments.Handlers;

public class UpdateAssignmentHandlerTests
{
    [Fact]
    public async Task UpdateAssignmentHandler_ValidData_ReturnsOkResultWithDataResponse()
    {
        // Arrange
        string accessorId = "0a161771-5a52-4ef5-a4c9-e1929d593ddd";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";
        string teamId = "7990d2fd-80ef-4885-9382-6bd703b7b1cc";
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";

        UpdateAssignmentRequest request = new()
        {
            Id = assignmentId,
            Name = "name",
            Description = "description",
            Status = AssignmentStatus.AwaitingProgress,
            ProjectId = projectId,
            DeveloperId = accessorId
        };

        Assignment assignment = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
            DeveloperId = accessorId
        };

        AssignmentModel assignmentResponseModel = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
            DeveloperId = accessorId
        };

        Project project = new()
        {
            Id = projectId,
            TeamId = teamId,
            Assignments = new List<Assignment> { assignment }
        };

        Team team = new()
        {
            Id = teamId,
            DeveloperIds = new List<string>() { accessorId, accessorId }
        };

        Mock<IMapper> mapperMock = new();
        mapperMock
            .Setup(m => m.Map<Assignment>(request))
            .Returns(assignment);

        mapperMock
            .Setup(m => m.Map<AssignmentModel>(assignment))
            .Returns(assignmentResponseModel);

        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));
        projectRepositoryMock
           .Setup(p => p.Update(project))
           .Returns(Task.FromResult(project));

        Mock<ITeamRepository> teamRepositoryMock = new();
        teamRepositoryMock
            .Setup(t => t.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(team));

        Mock<IExecutionContext> executionContextMock = new();
        executionContextMock
            .Setup(t => t.GetCurrentIdentity())
            .Returns(new CurrentUser { UserId = accessorId });

        UpdateAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, teamRepositoryMock.Object, executionContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        Ok<DataResponse<AssignmentModel>> okResult = Assert.IsType<Ok<DataResponse<AssignmentModel>>>(result);
        DataResponse<AssignmentModel> resultData = okResult.Value;
        AssignmentModel assignmentResponsePayload = resultData.Payload;
        Assert.Equivalent(assignmentResponseModel, assignmentResponsePayload, strict: true);
    }

    [Fact]
    public async Task UpdateAssignmentHandler_WithUnexistingProjectId_ReturnsBadRequestResultWithBaseResponse()
    {
        // Arrange
        string invalidProjectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6in";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";

        UpdateAssignmentRequest request = new()
        {
            Id = assignmentId,
            Name = "name",
            Description = "description",
            Status = AssignmentStatus.AwaitingProgress,
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
        Mock<IExecutionContext> executionContextMock = new();

        UpdateAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, teamRepositoryMock.Object, executionContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        BadRequest<BaseResponse> BadRequestResult = Assert.IsType<BadRequest<BaseResponse>>(result);
        BaseResponse resultData = BadRequestResult.Value;
        ResponseStatus assignmentResponseStatus = resultData.Status;
        Assert.Equivalent(expectedResponseStatus, assignmentResponseStatus, strict: true);
    }

    [Fact]
    public async Task UpdateAssignmentHandler_WithUnexistingAssignmentId_ReturnsBadRequestResultWithBaseResponse()
    {
        // Arrange
        string invalidProjectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6in";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";

        UpdateAssignmentRequest request = new()
        {
            Id = assignmentId,
            Name = "name",
            Description = "description",
            Status = AssignmentStatus.AwaitingProgress,
            ProjectId = invalidProjectId,
        };

        Project project = new();

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
        Mock<IExecutionContext> executionContextMock = new();

        UpdateAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, teamRepositoryMock.Object, executionContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        BadRequest<BaseResponse> BadRequestResult = Assert.IsType<BadRequest<BaseResponse>>(result);
        BaseResponse resultData = BadRequestResult.Value;
        ResponseStatus assignmentResponseStatus = resultData.Status;
        Assert.Equivalent(expectedResponseStatus, assignmentResponseStatus, strict: true);
    }

    [Fact]
    public async Task UpdateAssignmentHandler_AccessorIsNotProjectManagerAndProjectDoesNotHaveTeam_ReturnsUnauthorizedResult()
    {
        // Arrange
        string accessorId = "0a161771-5a52-4ef5-a4c9-e1929d593ddd";
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string developerId = "351fdfa7-b6c0-4e35-85c9-95ab20029472";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";

        UpdateAssignmentRequest request = new()
        {
            Id = assignmentId,
            Name = "name",
            Description = "description",
            Status = AssignmentStatus.AwaitingProgress,
            ProjectId = projectId,
            DeveloperId = developerId
        };

        Assignment assignment = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
        };

        Project project = new()
        {
            Id = projectId,
            Assignments = new List<Assignment> { assignment }
        };

        Mock<IMapper> mapperMock = new();
        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));
        Mock<ITeamRepository> teamRepositoryMock = new();
        teamRepositoryMock
            .Setup(t => t.GetSingle(It.IsAny<string>()))!
            .ReturnsAsync((Team)null);
        Mock<IExecutionContext> executionContextMock = new();
        executionContextMock
            .Setup(t => t.GetCurrentIdentity())
            .Returns(new CurrentUser { UserId = accessorId });

        UpdateAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, teamRepositoryMock.Object, executionContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        UnauthorizedHttpResult unauthorizedResult = Assert.IsType<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task UpdateAssignmentHandler_AccessorIsNotProjectManagerAndProjectHaveTeamButTaskIsNotAssignedToAccessor_ReturnsUnauthorizedResult()
    {
        // Arrange
        string accessorId = "0a161771-5a52-4ef5-a4c9-e1929d593ddd";
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string developerId = "351fdfa7-b6c0-4e35-85c9-95ab20029472";
        string teamId = "351fdfa7-b6c0-4e35-85c9-95ab20029123";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";

        UpdateAssignmentRequest request = new()
        {
            Id = assignmentId,
            Name = "name",
            Description = "description",
            Status = AssignmentStatus.AwaitingProgress,
            ProjectId = projectId,
            DeveloperId = accessorId
        };

        Assignment assignment = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
        };

        Team team = new()
        {
            Id = teamId,
            DeveloperIds = new List<string>() { developerId }
        };

        Project project = new()
        {
            Id = projectId,
            TeamId = teamId,
            Assignments = new List<Assignment> { assignment }
        };

        Mock<IMapper> mapperMock = new();
        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));
        Mock<ITeamRepository> teamRepositoryMock = new();
        teamRepositoryMock
            .Setup(t => t.GetSingle(It.IsAny<string>()))!
            .Returns(Task.FromResult(team));
        Mock<IExecutionContext> executionContextMock = new();
        executionContextMock
            .Setup(t => t.GetCurrentIdentity())
            .Returns(new CurrentUser { UserId = accessorId });

        UpdateAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, teamRepositoryMock.Object, executionContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        UnauthorizedHttpResult unauthorizedResult = Assert.IsType<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task UpdateAssignmentHandler_RequestDeveloperIdIsNotTeamMemberOrProjectManager_ReturnsBadRequestResultWithBaseResponse()
    {
        // Arrange
        string accessorId = "0a161771-5a52-4ef5-a4c9-e1929d593ddd";
        string projectId = "263ebf4c-52b2-4f9b-ac1d-e1f855bbc6be";
        string developerId = "351fdfa7-b6c0-4e35-85c9-95ab20029472";
        string teamId = "351fdfa7-b6c0-4e35-85c9-95ab20029123";
        string assignmentId = "8a896114-102d-45f1-8a8e-acf28973532a";

        UpdateAssignmentRequest request = new()
        {
            Id = assignmentId,
            Name = "name",
            Description = "description",
            Status = AssignmentStatus.AwaitingProgress,
            ProjectId = projectId,
            DeveloperId = developerId
        };

        Assignment assignment = new()
        {
            Id = assignmentId,
            Name = "test",
            Description = "test description",
            Status = AssignmentStatus.AwaitingProgress,
            DeveloperId = accessorId
        };

        Team team = new()
        {
            Id = teamId,
            DeveloperIds = new List<string>() { accessorId }
        };

        Project project = new()
        {
            Id = projectId,
            TeamId = teamId,
            Assignments = new List<Assignment> { assignment }
        };

        ResponseStatus expectedResponseStatus = new()
        {
            IsSuccess = false,
            Message = string.Format(ErrorMessages.UserIsNotTeamMember, request.DeveloperId)
        };

        Mock<IMapper> mapperMock = new();
        Mock<IProjectRepository> projectRepositoryMock = new();
        projectRepositoryMock
            .Setup(p => p.GetSingle(It.IsAny<string>()))
            .Returns(Task.FromResult(project));
        Mock<ITeamRepository> teamRepositoryMock = new();
        teamRepositoryMock
            .Setup(t => t.GetSingle(It.IsAny<string>()))!
            .Returns(Task.FromResult(team));
        Mock<IExecutionContext> executionContextMock = new();
        executionContextMock
            .Setup(t => t.GetCurrentIdentity())
            .Returns(new CurrentUser { UserId = accessorId });

        UpdateAssignmentHandler sut = new(mapperMock.Object, projectRepositoryMock.Object, teamRepositoryMock.Object, executionContextMock.Object);

        // Act

        IResult result = await sut.Handle(request, new CancellationToken());

        // Assert

        BadRequest<BaseResponse> BadRequestResult = Assert.IsType<BadRequest<BaseResponse>>(result);
        BaseResponse resultData = BadRequestResult.Value;
        ResponseStatus assignmentResponseStatus = resultData.Status;
        Assert.Equivalent(expectedResponseStatus, assignmentResponseStatus, strict: true);
    }
}
