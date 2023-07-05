using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.Identity.Abstractions;

public interface IIdentity
{
    Task<PaginatedResult<User>> SearchUserByUsername
        (string query, int pageSize, string? nextPageKey);
    Task<User> SearchUserById(string userId);
    Task<User?> CreateUser(string username, string email, string password);
}