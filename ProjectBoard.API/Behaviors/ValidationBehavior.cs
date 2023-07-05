using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest
    where TResponse : IResult
{
    private readonly IValidator<TRequest> _validator;

    public ValidationBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            string errorMessages = string.Format(string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage)));
            return (TResponse)Results.BadRequest(ResponseStatus.Error(errorMessages));
        }
        return await next();
    }
}   
