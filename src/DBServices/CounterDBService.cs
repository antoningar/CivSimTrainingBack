using cst_back.Models;
using cst_back.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace cst_back.DBServices
{
    public class CounterDBService : ICounterDBService
    {
        private readonly IMongoCollection<Counters> _countersCollection;
        public CounterDBService(IOptions<DatabaseSettings> dbSettings)
        {
            MongoClient mongoClient = new(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _countersCollection = mongoDatabase.GetCollection<Counters>(
                dbSettings.Value.CounterCollectionName);
        }

        private async Task<Counters> GetCountersAsync()
        {
            return await _countersCollection.Find(FilterDefinition<Counters>.Empty).FirstAsync();
        }

        private void SetCountersAsync(Counters counters)
        {
            _countersCollection.ReplaceOne(FilterDefinition<Counters>.Empty, counters, new ReplaceOptions() { IsUpsert = false });
        }

        public async Task<int?> GetUserIdCounterAsync()
        {
            Counters counters = await GetCountersAsync();
            return counters.userid_counter;
        }

        public async Task IncrementUserIdCounterAsync()
        {
            Counters counters = await GetCountersAsync();
            counters.userid_counter += 1;
            SetCountersAsync(counters);
        }
    }
}
