using FluentValidation;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Projects.Validation;
public class GetProjectValidator : AbstractValidator<GetProjectRequest>
{
    public GetProjectValidator()
    {
        RuleFor(x => x.Id)
            .Must(BeValidId)
            .WithMessage(x=> string.Format(ErrorMessages.InvalidInput, nameof(x.Id)))
            .NotEmpty()
            .NotNull();
    }
    private bool BeValidId(string? id)
    {
        return Guid.TryParse(id, out Guid result);
    }
}
