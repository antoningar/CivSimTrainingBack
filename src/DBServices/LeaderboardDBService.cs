using cst_back.Models;
using cst_back.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace cst_back.DBServices
{
    public class LeaderboardDBService : ILeaderboardDBService
    {
        private readonly IMongoCollection<Leaderboard> _leaderboardCollection;

        public LeaderboardDBService(IOptions<DatabaseSettings> dbSettings)
        {
            MongoClient mongoClient = new(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _leaderboardCollection = mongoDatabase.GetCollection<Leaderboard>(
                dbSettings.Value.LeaderboardCollectionName);
        }

        public async Task<Leaderboard?> GetLeaderboard(string id)
        {
            return await _leaderboardCollection.Find(x => x.InstanceId == id).FirstAsync();
        }
    }
}
