using FluentValidation;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Assignments.Validation;

public class GetAssignmentValidator : AbstractValidator<GetAssignmentRequest>
{
    public GetAssignmentValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .NotNull()
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.ProjectId)));

        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull()
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.Id)));
    }
}
