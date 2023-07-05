using ProjectBoard.API.Features.Users.Models;

namespace ProjectBoard.API.Features.Teams.Models;

public class TeamDetailsModel
{
    public string Id { get; set; } = String.Empty;

    public string Name { get; set; } = String.Empty;

    public List<UserModel> Developers { get; set; } = new List<UserModel>();

    public UserModel TeamLead { get; set; } = new UserModel();
}

