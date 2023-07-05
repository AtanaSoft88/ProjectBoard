using FluentValidation;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Assignments.Validation;

public class UpdateAssignmentValidator : AbstractValidator<UpdateAssignmentRequest>
{
    public UpdateAssignmentValidator()
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

        RuleFor(x => x.Name).NotEmpty().NotNull();

        RuleFor(x => x.Description).NotEmpty().NotNull();

        RuleFor(x => x.DeveloperId)
            .Must(x => Guid.TryParse(x, out _))
            .When(x => !string.IsNullOrEmpty(x.DeveloperId))
            .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.DeveloperId)));

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.DeveloperId)
            .Must(x => Guid.TryParse(x, out _))
            .When(x => !string.IsNullOrEmpty(x.DeveloperId))
            .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.DeveloperId)));
    }
}
