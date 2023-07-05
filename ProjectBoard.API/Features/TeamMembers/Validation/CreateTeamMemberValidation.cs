using FluentValidation;
using ProjectBoard.API.Features.TeamMembers.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.TeamMembers.Validation;

public class CreateTeamMemberValidation : AbstractValidator<CreateTeamMemberRequest>
{
    public CreateTeamMemberValidation()
    {
        RuleFor(x => x.TeamId)
                .NotEmpty()
                .WithMessage("TeamId field cannot be null or empty.")
                .Must(x => Guid.TryParse(x, out _))
                .WithMessage(ErrorMessages.InvalidTeamId);

        RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId field cannot be null or empty.")
                .Must(x => Guid.TryParse(x, out _))
                .WithMessage(ErrorMessages.InvalidUserId);
    }
}
