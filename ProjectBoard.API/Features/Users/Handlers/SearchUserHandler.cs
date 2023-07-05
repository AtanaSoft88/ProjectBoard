using MediatR;

using ProjectBoard.API.Utilities;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Features.Users.Requests;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Users.Handlers;

public class SearchUserHandler : IRequestHandler<SearchUserRequest, IResult>
{
    private readonly IIdentity _identity;

    public SearchUserHandler(IIdentity identity)
    {
        _identity = identity;
    }

    public async Task<IResult> Handle(SearchUserRequest request, CancellationToken cancellationToken)
    {
        List<UserModel> usersResponses = new();

        PaginatedResult<User> users = await _identity.SearchUserByUsername
            (request.Query, request.PageSize, request.NextPageKey);

        if (!users.Result.Any())
        {
            return Response.NotFound(ErrorMessages.UsernameNotFound);
        }

        foreach (User user in users.Result)
        {
            usersResponses.Add(new UserModel { Id = user.Id, Username = user.Username, Email = user.Email });
        }
        
        return Response.OkPage(usersResponses, request.PageSize, users.NextPageKey);
    }
}