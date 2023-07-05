using FluentValidation;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Teams.Validation;

public class GetAllTeamsValidator : AbstractValidator<GetAllTeamsRequest>
{
    public GetAllTeamsValidator()
    {
        RuleFor(x => x.NextPageKey)
            .Must(key => key == null || BeValidBase64(key))
            .WithMessage(ErrorMessages.InvalidNextPageKey);

        RuleFor(x => x.PageSize)
            .Custom((value, context) =>
            {
                if (value < Constants.MinPageSize || value > Constants.MaxPageSize)
                {
                    context.AddFailure(ErrorMessages.InvalidPageSize);
                }
            });
    }

    private bool BeValidBase64(string key)
    {
        if (key != null)
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

