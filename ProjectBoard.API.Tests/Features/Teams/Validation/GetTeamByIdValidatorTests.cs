using FluentValidation.TestHelper;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Features.Teams.Validation;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Tests.Features.Teams.Validation
{
    public class GetTeamByIdValidatorTests
    {
        private readonly GetTeamByIdValidator _validator;

        public GetTeamByIdValidatorTests()
        {
            _validator = new GetTeamByIdValidator();
        }

        [Fact]
        public async void GetTeamByIdValidator_TeamIdEmpty_ReturnsErrorMessage()
        {
            // Arrange
            var request = new GetTeamRequest { Id = string.Empty };

            // Act
            TestValidationResult<GetTeamRequest> result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(ErrorMessages.InvalidTeamId);
        }

        [Fact]
        public void GetTeamByIdValidator_ValidRequest_ShouldNotReturnError()
        {
            // Arrange
            var request = new GetTeamRequest { Id = "1bd5e395-944c-4930-86fc-7450d650b9db" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
