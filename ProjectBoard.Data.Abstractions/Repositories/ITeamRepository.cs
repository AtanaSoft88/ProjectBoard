using ProjectBoard.Data.Abstractions.Models;

namespace ProjectBoard.Data.Abstractions.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<Team> Save(Team teamDb);

        Task<Team> GetById(string teamId);       

        Task<PaginationResult<Team>> GetPaginatedTeams(int pageSize, string? pageKey);
    }
}
