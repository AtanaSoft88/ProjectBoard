using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Teams.Validation;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Tests.Features.Teams.Validation
{
    public class CreateTeamValidatorTests
    {
        private readonly CreateTeamValidator _validator;

        public CreateTeamValidatorTests()
        {
            _validator = new CreateTeamValidator();
        }

        [Fact]
        public async void CreateTeamValidator_ProjectIdEmpty_ReturnsErrorMessage()
        {
            // Arrange
            var request = new CreateTeamRequest { Name = string.Empty };

            // Act
            TestValidationResult<CreateTeamRequest> result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(t => t.Name);
        }

        [Fact]
        public void CreateTeamValidator_WhenNameIsTooShort_ReturnsErrorMessage()
        {
            // Arrange
            var request = new CreateTeamRequest { Name = "AB" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(t => t.Name)
                .WithErrorMessage("The length of 'Name' must be at least 3 characters. You entered 2 characters.");
        }

        [Fact]
        public void CreateTeamValidator_ValidRequest_ShouldNotReturnError()
        {
            // Arrange
            var request = new CreateTeamRequest { Name = "TeamA" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(t => t.Name);
        }
        
        [Fact]
        public void CreateTeamValidator_WhenTeamLeadIdIsEmpty_ReturnsErrorMessage()
        {
            // Arrange
            var request = new CreateTeamRequest { TeamLeadId = string.Empty };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TeamLeadId)
                .WithErrorMessage("'Team Lead Id' must not be empty.");
        }

        [Fact]
        public void CreateTeamValidator_WhenTeamLeadIdIsInvalidGuid_ReturnsErrorMessage()
        {
            // Arrange
            var request = new CreateTeamRequest { TeamLeadId = "123" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TeamLeadId)
                .WithErrorMessage(ErrorMessages.InvalidTeamLeadId);
        }

        [Fact]
        public void CreateTeamValidator_WhenTeamLeadIdIsValidGuid_ShouldNotReturnError()
        {
            // Arrange
            var request = new CreateTeamRequest { TeamLeadId = Guid.NewGuid().ToString() };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TeamLeadId);
        }
    }
}
