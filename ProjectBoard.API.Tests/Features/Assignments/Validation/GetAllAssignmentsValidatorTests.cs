using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Assignments.Validation;

namespace ProjectBoard.API.Tests.Features.Assignments.Validation;

public class GetAllAssignmentsValidatorTests
{
    private readonly GetAllAssignmentsValidator validator;

    public GetAllAssignmentsValidatorTests()
    {
        validator = new GetAllAssignmentsValidator();
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_ProjectIdEmpty_ShouldHaveValidationErrorAsync()
    {
        GetAllAssignmentsRequest request = new() { ProjectId = "" };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_ProjectIdNull_ShouldHaveValidationErrorAsync()
    {
        GetAllAssignmentsRequest request = new() { ProjectId = null };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_ProjectIdInvalidGuid_ShouldHaveValidationErrorAsync()
    {
        GetAllAssignmentsRequest request = new() { ProjectId = "invalid-guid" };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_ProjectIdValidGuid_ShouldNotHaveValidationErrorAsync()
    {
        GetAllAssignmentsRequest request = new() { ProjectId = Guid.NewGuid().ToString() };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_PageSizeZero_ShouldHaveValidationError()
    {
        GetAllAssignmentsRequest request = new() { PageSize = 0 };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_PageSizeNegative_ShouldHaveValidationError()
    {
        GetAllAssignmentsRequest request = new() { PageSize = -10 };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_PageSizePositive_ShouldNotHaveValidationError()
    {
        GetAllAssignmentsRequest request = new() { PageSize = 10 };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_NextPageKeyZero_ShouldHaveValidationError()
    {
        GetAllAssignmentsRequest request = new() { NextPageKey = "0" };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.NextPageKey);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_NextPageKeyNegative_ShouldHaveValidationError()
    {
        GetAllAssignmentsRequest request = new() { NextPageKey = "-10" };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(x => x.NextPageKey);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_NextPageKeyPositive_ShouldNotHaveValidationError()
    {
        GetAllAssignmentsRequest request = new() { NextPageKey = "10" };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.NextPageKey);
    }

    [Fact]
    public async Task GetAllAssignmentsValidator_NextPageKeyNull_ShouldNotHaveValidationError()
    {
        GetAllAssignmentsRequest request = new() { NextPageKey = null };
        TestValidationResult<GetAllAssignmentsRequest> result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveValidationErrorFor(x => x.NextPageKey);
    }
}