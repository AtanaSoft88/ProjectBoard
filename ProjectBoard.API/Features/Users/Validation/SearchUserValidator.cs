using FluentValidation;

using ProjectBoard.API.Utilities;
using ProjectBoard.API.Features.Users.Requests;

namespace ProjectBoard.API.Features.Users.Validation;

public class SearchUserValidator : AbstractValidator<SearchUserRequest>
{
    public SearchUserValidator()
    {
        RuleFor(x => x.Query)
            .MinimumLength(3)
            .WithMessage(ErrorMessages.UsernameMinLengthErrorMessage);

        RuleFor(x => x.Query)
            .Matches("^[0-9a-zA-Z]+$")
            .WithMessage(ErrorMessages.UsernameSpecialSymbolError);

        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(60)
            .WithMessage(ErrorMessages.PageSizeMaxValueError);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage(ErrorMessages.PageSizeMinValueError);
            
        RuleFor(x => x.NextPageKey)
            .Must(key => key is null || BeValidBase64(key))
            .WithMessage(ErrorMessages.InvalidNextPageKey);
    }

    private bool BeValidBase64(string key)
    {
        if (key is not null)
        {
            try
            {
                Convert.FromBase64String(key);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        return true;
    }
}