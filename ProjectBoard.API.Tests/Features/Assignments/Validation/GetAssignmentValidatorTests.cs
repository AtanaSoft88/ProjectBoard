using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Assignments.Validation;

namespace ProjectBoard.API.Tests.Features.Assignments.Validation;

public class GetAssignmentValidatorTests
{
    private readonly GetAssignmentValidator validator;

    public GetAssignmentValidatorTests()
    {
        validator = new GetAssignmentValidator();
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_ProjectIdEmpty_ShouldHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { ProjectId = "" };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_ProjectIdNull_ShouldHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { ProjectId = null };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_ProjectIdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { ProjectId = "invalid-guid" };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_ProjectIdValidGuid_ShouldNotHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { ProjectId = Guid.NewGuid().ToString() };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_IdEmpty_ShouldHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { Id = "" };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_IdNull_ShouldHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { Id = null };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_IdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { Id = "invalid-guid" };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task GetAssignmentByIdValidator_IdValidGuid_ShouldNotHaveValidationErrorAsync()
    {
        GetAssignmentRequest request = new() { Id = Guid.NewGuid().ToString() };
        TestValidationResult<GetAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
