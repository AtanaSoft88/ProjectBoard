using FluentValidation;
using ProjectBoard.API.Features.Teams.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Teams.Validation;

public class GetTeamByIdValidator : AbstractValidator<GetTeamRequest>
{
    public GetTeamByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage(ErrorMessages.InvalidTeamId);
    }
}


