using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Validation;
using System.Text;

namespace ProjectBoard.API.Tests.Features.Projects.Validation;

public class GetAllProjectsValidatorTests
{
    private GetAllProjectsValidator _validator;
    public GetAllProjectsValidatorTests()
    {
        _validator = new GetAllProjectsValidator();
    }
    [Fact]
    public async Task GetAllProjectsValidator_InvalidBase64KeyFormat_ShouldReturnErrors()
    {
        //Arrange
        byte[] bytes = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
        string key = Convert.ToBase64String(bytes);
        var request = new GetAllProjectsRequest()
        {
            PageSize = 10,
            NextPageKey = $"@{key}@",
        };

        //Act            
        TestValidationResult<GetAllProjectsRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.NextPageKey);
        Assert.True(result.IsValid == false);
    }

    [Theory]
    [InlineData("Yjk0YjFjOTEtODViOC00NGEzLTkzOWQtOTQ2MjY0N2U1Yzdi")]
    [InlineData(null)]
    public async Task GetAllProjectsValidator_OnValidBase64EncodingFormat_ShouldNotReturnErrors(string key)
    {
        //Arrange            
        var request = new GetAllProjectsRequest()
        {
            PageSize = 10,
            NextPageKey = key,
        };

        //Act            
        TestValidationResult<GetAllProjectsRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldNotHaveValidationErrorFor(x => x.NextPageKey);
        Assert.True(result.IsValid == true);
    }

    // Write here tests for PageSize
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(61)]
    public async Task GetAllProjectsValidator_OnInvalidPageSize_ShouldReturnErrors(int pageSize)
    {
        //Arrange
        string nextPageKey = "Yjk0YjFjOTEtODViOC00NGEzLTkzOWQtOTQ2MjY0N2U1Yzdi";
        var request = new GetAllProjectsRequest()
        {
            PageSize = pageSize,
            NextPageKey = nextPageKey
        };

        //Act            
        TestValidationResult<GetAllProjectsRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
        Assert.True(result.IsValid == false);
    }

    [Theory]
    [InlineData(1)]        
    [InlineData(60)]
    public async Task GetAllProjectsValidator_OnValidPageSize_ShouldReturnErrors(int pageSize)
    {
        //Arrange
        string nextPageKey = "Yjk0YjFjOTEtODViOC00NGEzLTkzOWQtOTQ2MjY0N2U1Yzdi";
        var request = new GetAllProjectsRequest()
        {
            PageSize = pageSize,
            NextPageKey = nextPageKey
        };

        //Act            
        TestValidationResult<GetAllProjectsRequest> result = await _validator.TestValidateAsync(request);

        //Assert            
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
        Assert.True(result.IsValid == true);
    }
}
