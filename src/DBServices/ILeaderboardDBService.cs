namespace cst_back.DBServices
{
    public interface ILeaderboardDBService
    {
        public Task<Models.Leaderboard?> GetLeaderboard(string id);
    }
}
