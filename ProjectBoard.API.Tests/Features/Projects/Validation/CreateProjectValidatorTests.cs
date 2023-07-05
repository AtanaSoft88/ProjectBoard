using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Validation;
using ProjectBoard.Data.Abstractions.Enums;
using FluentValidation.TestHelper;

namespace ProjectBoard.API.Tests.Features.Projects.Validation;

public class CreateProjectValidatorTests
{
    private CreateProjectValidator _validator;
    public CreateProjectValidatorTests()
    {
        _validator = new CreateProjectValidator();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    public async Task CreateProjectValidator_WhenNameAndDescriptionParametersAreNullOrEmpty_ReturnsErrors(string name, string description)
    {
        //Arrange
        var request = new CreateProjectRequest()
        {
            Name = name,
            Status = ProjectStatus.InProgress,
            Description = description,
            ProjectManagerId = Guid.NewGuid().ToString(),
        };

        //Act            
        TestValidationResult<CreateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);            
        Assert.True(result.IsValid == false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateProjectValidator_TeamIdCanBeNullOrEmpty_WithoutAnyErrors(string teamId)
    {
        //Arrange
        var request = new CreateProjectRequest()
        {
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = Guid.NewGuid().ToString(),
            TeamId = teamId
        };

        //Act            
        TestValidationResult<CreateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldNotHaveValidationErrorFor(x => x.TeamId);
        Assert.True(result.IsValid == true);
    }

    [Theory]
    [InlineData("1 2 3")]
    [InlineData("     ")]
    [InlineData("asdfgh")]
    public async Task CreateProjectValidator_WhenTeamIdIsNotValidGuid_ReturnsErrors(string teamId)
    {
        //Arrange
        var request = new CreateProjectRequest()
        {
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = Guid.NewGuid().ToString(),
            TeamId = teamId
        };

        //Act            
        TestValidationResult<CreateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.TeamId);
        Assert.True(result.IsValid == false);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateProjectValidator_ProjectManagerIdCanNotBeNullOrEmpty_ReturnsErrors(string projectManagerId)
    {
        //Arrange
        var request = new CreateProjectRequest()
        {
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = projectManagerId,
            TeamId = Guid.NewGuid().ToString()
        };

        //Act            
        TestValidationResult<CreateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.ProjectManagerId);
        Assert.True(result.IsValid == false);
    }

    [Theory]
    [InlineData("1 2 3")]
    [InlineData("     ")]
    [InlineData("asdfgh")]
    public async Task CreateProjectValidator_WhenProjectManagerIdIsNotValidGuid_ReturnsErrors(string projectManagerId)
    {
        //Arrange
        var request = new CreateProjectRequest()
        {
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = projectManagerId,
            TeamId = Guid.NewGuid().ToString()
        };

        //Act            
        TestValidationResult<CreateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.ProjectManagerId);
        Assert.True(result.IsValid == false);
    }        
}
