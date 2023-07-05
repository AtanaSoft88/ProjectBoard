using FluentValidation;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Assignments.Validation;

public class GetAllAssignmentsValidator : AbstractValidator<GetAllAssignmentsRequest>
{
    public GetAllAssignmentsValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .NotNull()
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.ProjectId)));

        RuleFor(x => x.PageSize)
            .Custom((value, context) =>
            {
                if (value < 1 || value > 60)
                {
                    context.AddFailure(ErrorMessages.InvalidPageSize);
                }
            });

        RuleFor(x => x.NextPageKey)
            .Must(IsPropertyValueBiggerThanZero)
            .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.NextPageKey)));
    }

    private bool IsPropertyValueBiggerThanZero(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        int valueAsInteger = 0;
        bool isValueIsInteger = int.TryParse(value, out valueAsInteger);

        if (!isValueIsInteger || valueAsInteger <= 0)
        {
            return false;
        }

        return true;
    }
}
