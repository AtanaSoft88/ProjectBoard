using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.Data.Abstractions.Enums;

namespace ProjectBoard.API.Features.Projects.Models
{
    public class ProjectDetailsModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public UserModel ProjectManager { get; set; } = new UserModel();
        public List<AssignmentModel> Assignments { get; set; } = new List<AssignmentModel>();
        public string? TeamId { get; set; }
    }
}
