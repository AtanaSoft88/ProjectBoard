using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Assignments.Validation;
using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.API.Tests.Features.Assignments.Validation;

public class CreateAssignmentValidatorTests
{
    private readonly CreateAssignmentValidator validator;

    public CreateAssignmentValidatorTests()
    {
        validator = new CreateAssignmentValidator();
    }

    [Fact]
    public async Task CreateAssignmentValidator_ProjectIdEmpty_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { ProjectId = "" };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task CreateAssignmentValidator_ProjectIdNull_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { ProjectId = null };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task CreateAssignmentValidator_ProjectIdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { ProjectId = "invalid-guid" };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task CreateAssignmentValidator_ProjectIdValidGuid_ShouldNotHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { ProjectId = Guid.NewGuid().ToString() };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task CreateAssignmentValidator_NameEmpty_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { Name = "" };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task CreateAssignmentValidator_NameNull_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { Name = null };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task CreateAssignmentValidator_DescriptionEmpty_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { Description = "" };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task CreateAssignmentValidator_DescriptionNull_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { Description = null };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task CreateAssignmentValidator_DeveloperIdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { DeveloperId = "invalid-guid" };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.DeveloperId);
    }

    [Fact]
    public async Task CreateAssignmentValidator_DeveloperIdNull_ShouldNotHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { DeveloperId = null };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.DeveloperId);
    }

    [Fact]
    public async Task CreateAssignmentValidator_StatusInvalidEnumValue_ShouldHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { Status = (AssignmentStatus)100 };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public async Task CreateAssignmentValidator_StatusValidEnumValue_ShouldNotHaveValidationErrorAsync()
    {
        CreateAssignmentRequest request = new() { Status = AssignmentStatus.InProgress };
        TestValidationResult<CreateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }
}
