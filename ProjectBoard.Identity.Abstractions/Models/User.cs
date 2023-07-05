namespace ProjectBoard.Identity.Abstractions.Models;

public class User
{
    public string Id { get; private set; }

    public string Username { get; private set; }

    public string Email { get; private set; }

    public User(string id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }
}