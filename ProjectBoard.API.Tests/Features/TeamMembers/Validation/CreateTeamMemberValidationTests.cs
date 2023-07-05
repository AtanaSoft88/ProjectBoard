using FluentValidation.Results;
using ProjectBoard.API.Features.TeamMembers.Requests;
using ProjectBoard.API.Features.TeamMembers.Validation;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Tests.Features.TeamMembers.Validation;

public class CreateTeamMemberValidationTests
{
    [Fact]
    public void Validate_WhenTeamIdIsNull_ShouldReturnValidationError()
    {
        // Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest
        {
            TeamId = null,
            UserId = Guid.NewGuid().ToString(),
        };

        CreateTeamMemberValidation sut = new CreateTeamMemberValidation();

        // Act
        ValidationResult result = sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("TeamId field cannot be null or empty.", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_WhenTeamIdIsWrongGuid_ShouldReturnValidationError()
    {
        // Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest
        {
            TeamId = "WrongGuid",
            UserId = Guid.NewGuid().ToString(),
        };

        CreateTeamMemberValidation sut = new CreateTeamMemberValidation();

        // Act
        ValidationResult result = sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ErrorMessages.InvalidTeamId, result.Errors[0].ErrorMessage);
    }

    public void Validate_WhenUserIdIsNull_ShouldReturnValidationError()
    {
        // Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest
        {
            TeamId = Guid.NewGuid().ToString(),
            UserId = null,
        };

        CreateTeamMemberValidation sut = new CreateTeamMemberValidation();

        // Act
        ValidationResult result = sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("TeamId field cannot be null or empty.", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_WhenUserIdIsWrongGuid_ShouldReturnValidationError()
    {
        // Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest
        {
            TeamId = Guid.NewGuid().ToString(),
            UserId = "WrongGuid",
        };

        CreateTeamMemberValidation sut = new CreateTeamMemberValidation();

        // Act
        ValidationResult result = sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ErrorMessages.InvalidUserId, result.Errors[0].ErrorMessage);
    }
    [Fact]
    public void Validate_WhenBothAreValid_ShouldNotReturnError()
    {
        // Arrange
        CreateTeamMemberRequest request = new CreateTeamMemberRequest
        {
            TeamId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
        };

        CreateTeamMemberValidation sut = new CreateTeamMemberValidation();

        // Act
        ValidationResult result = sut.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
