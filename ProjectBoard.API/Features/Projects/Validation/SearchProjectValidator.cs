using FluentValidation;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Utilities;

namespace ProjectBoard.API.Features.Projects.Validation
{
    public class SearchProjectValidator : AbstractValidator<SearchProjectRequest>
    {
        public SearchProjectValidator()
        {
            RuleFor(x => x.NextPageKey)
            .Must(key => key == null || BeValidBase64(key))
            .WithMessage(ErrorMessages.InvalidNextPageKey);

            RuleFor(x => x.PageSize)
                .Custom((value, context) =>
                {
                    if (value == null)
                    {
                        context.RootContextData[context.PropertyName] = 10;
                        return;
                    }
                    if (value < 1 || value > 60)
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
}
