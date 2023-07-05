using FluentValidation;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Featuress.Validation
{
    public class UpdateProjectValidator : AbstractValidator<UpdateProjectRequest>
    {       
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Id)
                .Must(BeValidId)
                .When(x => IsNotNull(x.Id))
                .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.Id)))
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.ProjectManagerId)
                .Must(BeValidId)
                .When(x => IsNotNull(x.ProjectManagerId))
                .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.ProjectManagerId)))
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.TeamId)
                .Must(BeValidId)
                .When(x => IsNotNull(x.TeamId))
                .WithMessage(x => string.Format(ErrorMessages.InvalidInput, nameof(x.TeamId)));                

            RuleFor(x => x.Description)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Status)
                .IsInEnum();

        }
        private bool BeValidId(string? id)
        {
            return Guid.TryParse(id, out Guid result);
        }
        private bool IsNotNull(string input)
        {
            return !string.IsNullOrEmpty(input);
        }
    }
}
