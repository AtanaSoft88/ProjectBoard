using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Assignments.Validation;
using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.API.Tests.Features.Assignments.Validation;

public class UpdateAssignmentValidatorTests
{
    private readonly UpdateAssignmentValidator validator;

    public UpdateAssignmentValidatorTests()
    {
        validator = new UpdateAssignmentValidator();
    }

    [Fact]
    public async Task UpdateAssignmentValidator_ProjectIdEmpty_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { ProjectId = "" };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_ProjectIdNull_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { ProjectId = null };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_ProjectIdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { ProjectId = "invalid-guid" };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_ProjectIdValidGuid_ShouldNotHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { ProjectId = Guid.NewGuid().ToString() };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_IdEmpty_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Id = "" };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_IdNull_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Id = null };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_IdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Id = "invalid-guid" };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_IdValidGuid_ShouldNotHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Id = Guid.NewGuid().ToString() };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_NameEmpty_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Name = "" };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_NameNull_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Name = null };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_DescriptionEmpty_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Description = "" };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_DescriptionNull_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Description = null };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_DeveloperIdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { DeveloperId = "invalid-guid" };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.DeveloperId);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_DeveloperIdValidGuid_ShouldNotHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { DeveloperId = Guid.NewGuid().ToString() };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.DeveloperId);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_StatusInvalidEnumValue_ShouldHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Status = (AssignmentStatus)100 };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public async Task UpdateAssignmentValidator_StatusValidEnumValue_ShouldNotHaveValidationErrorAsync()
    {
        UpdateAssignmentRequest request = new() { Status = AssignmentStatus.InProgress };
        TestValidationResult<UpdateAssignmentRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }
}
