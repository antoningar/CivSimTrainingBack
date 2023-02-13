using cst_back.Models;

namespace cst_back.DBServices
{
    public interface ILeaderboardDBService
    {
        public Task<Leaderboard?> GetLeaderboard(string id);
        public Task InsertLeaderboard(Leaderboard leaderboard);
    }
}
