using FluentValidation;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Teams.Validation;

public class CreateTeamValidator : AbstractValidator<CreateTeamRequest>
{
    public CreateTeamValidator()
    {
        RuleFor(t => t.Name).NotEmpty().MinimumLength(3);

        RuleFor(x => x.TeamLeadId)
          .NotEmpty()
          .Must(x => Guid.TryParse(x, out _))
          .WithMessage(ErrorMessages.InvalidTeamLeadId);
    }
}

