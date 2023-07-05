using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Teams.Validation;
using ProjectBoard.API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoard.API.Tests.Features.Teams.Validation
{
    public class UpdateTeamValidatorTests
    {
        private readonly UpdateTeamValidator _validator;

        public UpdateTeamValidatorTests()
        {
            _validator = new UpdateTeamValidator();
        }

        [Fact]
        public async void UpdateTeamValidation_TeamIdEmpty_ReturnsErrorMessage()
        {
            // Arrange
            var request = new UpdateTeamRequest { Id = string.Empty };

            // Act
            TestValidationResult<UpdateTeamRequest> result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(ErrorMessages.InvalidTeamId);
        }

        [Fact]
        public void UpdateTeamValidation_WhenNameIsTooShort_ReturnsErrorMessage()
        {
            // Arrange
            var request = new UpdateTeamRequest { Name = "AB" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(t => t.Name)
                .WithErrorMessage("The length of 'Name' must be at least 3 characters. You entered 2 characters.");
        }

        [Fact]
        public void UpdateTeamValidation_ValidRequest_ShouldNotReturnError()
        {
            // Arrange
            var request = new UpdateTeamRequest { Id = "1bd5e395-944c-4930-86fc-7450d650b9db", Name = "TeamA", TeamLeadId = "5d9bf09d-5c75-425d-8a2f-031034639f86" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void UpdateTeamValidation_WhenTeamLeadIdIsEmpty_ReturnsErrorMessage()
        {
            // Arrange
            var request = new UpdateTeamRequest { TeamLeadId = string.Empty };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TeamLeadId)
                .WithErrorMessage("'Team Lead Id' must not be empty.");
        }

        [Fact]
        public void UpdateTeamValidation_WhenTeamLeadIdIsInvalidGuid_ReturnsErrorMessage()
        {
            // Arrange
            var request = new UpdateTeamRequest { TeamLeadId = "123" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TeamLeadId)
                .WithErrorMessage(ErrorMessages.InvalidTeamLeadId);
        }

        [Fact]
        public void UpdateTeamValidation_WhenTeamLeadIdIsValidGuid_ShouldNotReturnError()
        {
            // Arrange
            var request = new UpdateTeamRequest { TeamLeadId = Guid.NewGuid().ToString() };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TeamLeadId);
        }
    }
}
