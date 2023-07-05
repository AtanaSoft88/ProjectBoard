using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Featuress.Validation;
using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.API.Tests.Features.Projects.Validation;

public class UpdateProjectVaidatorTests
{
    private UpdateProjectValidator _validator;
    public UpdateProjectVaidatorTests()
    {
        _validator = new UpdateProjectValidator();
    }
        
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123456")]
    public async Task UpdateProjectValidator_InvalidParameterForId_ReturnsErrors(string id)
    {
        //Arrange
        var request = new UpdateProjectRequest()
        {
            Id = id,
            ProjectManagerId = Guid.NewGuid().ToString(),
            Name = "Name",
            Description = "Description",    
            TeamId = Guid.NewGuid().ToString(),
            Status = ProjectStatus.InProgress
            
        };

        //Act            
        TestValidationResult<UpdateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.Id);      
        Assert.True(result.IsValid == false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateProjectValidator_TeamIdCanBeNullOrEmpty_WithoutAnyErrors(string teamId)
    {
        //Arrange
        var request = new UpdateProjectRequest()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = Guid.NewGuid().ToString(),
            TeamId = teamId,            
        };

        //Act            
        TestValidationResult<UpdateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldNotHaveValidationErrorFor(x => x.TeamId);
        Assert.True(result.IsValid == true);
    }

    [Theory]
    [InlineData("1 2 3")]
    [InlineData("     ")]
    [InlineData("asdfgh")]
    public async Task UpdateProjectValidator_WhenTeamIdIsNotValidGuid_ReturnsErrors(string teamId)
    {
        //Arrange
        var request = new UpdateProjectRequest()
        {
            Id= Guid.NewGuid().ToString(),
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = Guid.NewGuid().ToString(),
            TeamId = teamId
        };

        //Act            
        TestValidationResult<UpdateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.TeamId);
        Assert.True(result.IsValid == false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateProjectValidator_WhenProjectManagerIdIsNullOrEmpty_ReturnsErrors(string projectManagerId)
    {
        //Arrange
        var request = new UpdateProjectRequest()
        {
            Id= Guid.NewGuid().ToString(),
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = projectManagerId,
            TeamId = Guid.NewGuid().ToString()
        };

        //Act            
        TestValidationResult<UpdateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.ProjectManagerId);
        Assert.True(result.IsValid == false);
    }

    [Theory]
    [InlineData("1 2 3")]
    [InlineData("     ")]
    [InlineData("asdfgh")]
    public async Task UpdateProjectValidator_WhenProjectManagerIdIsNotValidGuid_ReturnsErrors(string projectManagerId)
    {
        //Arrange
        var request = new UpdateProjectRequest()
        {   
            Id = Guid.NewGuid().ToString(),
            Name = "TestName",
            Status = ProjectStatus.InProgress,
            Description = "Desc-Test",
            ProjectManagerId = projectManagerId,
            TeamId = Guid.NewGuid().ToString()
        };

        //Act            
        TestValidationResult<UpdateProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.ProjectManagerId);
        Assert.True(result.IsValid == false);
    }
}
