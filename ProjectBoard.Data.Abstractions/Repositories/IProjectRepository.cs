using ProjectBoard.Data.Abstractions.Models;

namespace ProjectBoard.Data.Abstractions.Repositories;
public interface IProjectRepository : IRepository<Project>
{
    Task<PaginationResult<Project>> GetPaginatedProjects(int pageSize, string? pageKey);
    Task<PaginationResult<Project>> SearchProjectByName(string name, int pageSize, string? pageKey);
}
