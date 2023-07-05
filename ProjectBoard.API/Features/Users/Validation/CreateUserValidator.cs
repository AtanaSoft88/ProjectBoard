using FluentValidation;
using ProjectBoard.API.Features.Users.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Users.Validation
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {        
        public CreateUserValidator()
        {
            RuleFor(x=>x.Username)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Email)
                .EmailAddress();                

            RuleFor(x => x.Password)                
                .Matches(x => ErrorMessages.PasswordRegex)
                .WithMessage(ErrorMessages.PasswordRequirementsMismatch);
        }
    }
}
