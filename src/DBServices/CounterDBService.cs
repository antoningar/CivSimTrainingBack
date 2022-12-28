using cst_back.Models;
using cst_back.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;

namespace cst_back.DBServices
{
    public class CounterDBService : ICounterDBService
    {
        private readonly IMongoCollection<Counters> _countersCollection;
        public CounterDBService(IOptions<AccountDatabaseSettings> accountSettings)
        {
            MongoClient mongoClient = new(accountSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(accountSettings.Value.DatabaseName);

            _countersCollection = mongoDatabase.GetCollection<Counters>(
                accountSettings.Value.CounterCollectionName);
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
