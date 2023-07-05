using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Validation;
namespace ProjectBoard.API.Tests.Features.Projects.Validation;

public class GetProjectValidatorTests
{
    private GetProjectValidator _validator;
    public GetProjectValidatorTests()
    {
        _validator = new GetProjectValidator();
    }
    [Theory]
    [InlineData("DDDDD-e89b-12d3-a456-aaa@")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123456")]
    public async Task GetProjectValidator_InvalidProjectId_ShouldReturnErrors(string id) 
    {
        //Arrange
        var request = new GetProjectRequest()
        {
            Id = id
        };

        //Act            
        TestValidationResult<GetProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.Id);
        Assert.True(result.IsValid == false);
    }

    [Fact]
    public async Task GetProjectValidator_ValidProjectId_ShouldPassWithoutErrors()
    {
        //Arrange
        var request = new GetProjectRequest()
        {
            Id = Guid.NewGuid().ToString(),
        };

        //Act            
        TestValidationResult<GetProjectRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        Assert.True(result.IsValid == true);
    }
}
