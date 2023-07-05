using Microsoft.Extensions.Options;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using ProjectBoard.Identity.Utilities;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.Identity;

public class CognitoIdentity : IIdentity
{
    private readonly IAmazonCognitoIdentityProvider _identityManager;
    private readonly IdentityOptions _identityOptions;

    public CognitoIdentity(IAmazonCognitoIdentityProvider identityManager, IOptions<IdentityOptions> identityOptions)
    {
        _identityManager = identityManager;
        _identityOptions = identityOptions.Value;
    }

    public async Task<User?> CreateUser(string username, string email, string password)
    {
        bool usernameExists = await FindUserByUserName(_identityManager, _identityOptions.UserPoolId, username);
        bool emailExists = await FindUserByEmail(_identityManager, _identityOptions.UserPoolId, email);  
        if (usernameExists || emailExists)
        {
            return null;
        }
        var request = new AdminCreateUserRequest
        {
            UserPoolId = _identityOptions.UserPoolId,
            Username = username,
            TemporaryPassword = password,
            UserAttributes = new List<AttributeType>
            {
                new AttributeType { Name = "email", Value = email },
            }
        };
        try
        {
            AdminCreateUserResponse response = await _identityManager.AdminCreateUserAsync(request);
            string? userId = response.User.Attributes.FirstOrDefault(attr => attr.Name == Constants.IdAttribute)?.Value;
            var user = new User(userId, username, email);
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format(Constants.UserCreationFailure, ex.Message));
        }
    }
    async Task<bool> FindUserByUserName(IAmazonCognitoIdentityProvider identityManager, string poolId, string username)
    {
        ListUsersRequest request = new ListUsersRequest
        {
            UserPoolId = poolId,
            Filter = $"username = \"{username}\"",
            Limit = 1
        };

        ListUsersResponse response = await identityManager.ListUsersAsync(request);
        return response.Users.Count > 0;
    }

    async Task<bool> FindUserByEmail(IAmazonCognitoIdentityProvider identityManager, string poolId, string email)
    {
        ListUsersRequest request = new ListUsersRequest
        {
            UserPoolId = poolId,
            Filter = $"email = \"{email}\"",
            Limit = 1
        };

        ListUsersResponse response = await identityManager.ListUsersAsync(request);
        return response.Users.Count > 0;
    }

    public async Task<PaginatedResult<User>> SearchUserByUsername
        (string searchQuery, int pageSize, string? nextPageKey)
    {
        List<User> usersToReturn = new();
        ListUsersResponse usersFromCognito = new();
        PaginatedResult<User> paginatedResult = new();

        ListUsersRequest request = new()
        {
            UserPoolId = _identityOptions.UserPoolId,
            Filter = string.Format(Constants.SearchUserFilter, searchQuery),
            Limit = pageSize,
            PaginationToken = nextPageKey
        };

        usersFromCognito = await _identityManager.ListUsersAsync(request);

        foreach (var user in usersFromCognito.Users)
        {
            User currentUser = new
            (
                GetAttributeValue(user, Constants.IdAttribute)!,
                user.Username,
                GetAttributeValue(user, Constants.EmailAttribute)!
            );

            usersToReturn.Add(currentUser);
        }

        paginatedResult.Result = usersToReturn;
        paginatedResult.NextPageKey = usersFromCognito.PaginationToken;

        return paginatedResult;
    }

    public async Task<User> SearchUserById(string userId)
    {
        User currentUser = null;
        ListUsersRequest request = new()
        {
            UserPoolId = _identityOptions.UserPoolId,
            Filter = string.Format(Constants.SearchUserIdFilter, userId)
        };

        ListUsersResponse usersFromCognito = await _identityManager.ListUsersAsync(request);

        if (usersFromCognito.Users.Count > 0)
        {
            currentUser = new
            User(
                GetAttributeValue(usersFromCognito.Users[0], Constants.IdAttribute)!,
                usersFromCognito.Users[0].Username,
                GetAttributeValue(usersFromCognito.Users[0], Constants.EmailAttribute)!
            );
            return currentUser;
        }

        return currentUser;
    }

    private static string? GetAttributeValue(UserType user, string attributeName)
    {
        AttributeType? value = user.Attributes.FirstOrDefault(a => a.Name == attributeName);
        return value?.Value;
    }
}