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
    public class GetAllTeamsValidatorTests
    {
        private readonly GetAllTeamsValidator _validator;

        public GetAllTeamsValidatorTests()
        {
            _validator = new GetAllTeamsValidator();
        }

        [Fact]
        public async void GetAllTeamsValidator_InvalidNextPageKey_ReturnsErrorMessage()
        {
            // Arrange
            var request = new GetAllTeamsRequest { NextPageKey = "invalid-key" };

            // Act
            TestValidationResult<GetAllTeamsRequest> result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NextPageKey)
                .WithErrorMessage(ErrorMessages.InvalidNextPageKey);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1000)]
        public void GetAllTeamsValidator_InvalidPageSize_ReturnsErrorMessage(int pageSize)
        {
            // Arrange
            var request = new GetAllTeamsRequest { PageSize = pageSize };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize)
                .WithErrorMessage(ErrorMessages.InvalidPageSize);
        }

        [Fact]
        public void GetAllTeamsValidator_ValidPageSize_ReturnsNoError()
        {
            // Arrange
            var request = new GetAllTeamsRequest { PageSize = 20 };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async void GetAllTeamsValidator_ValidNextPageKey_ReturnsNoError()
        {
            // Arrange
            var request = new GetAllTeamsRequest { NextPageKey = null };

            // Act
            TestValidationResult<GetAllTeamsRequest> result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
