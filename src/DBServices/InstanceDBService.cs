using cst_back.Models;
using cst_back.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace cst_back.DBServices
{
    public class InstanceDBService : IInstanceDBService
    {
        private readonly IMongoCollection<Instance> _instanceCollection;
        public InstanceDBService(IOptions<DatabaseSettings> dbSettings)
        {
            MongoClient mongoClient = new(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _instanceCollection = mongoDatabase.GetCollection<Instance>(
                dbSettings.Value.InstanceCollectionName);
        }
        public async Task<List<Instance>> GetInstances(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await _instanceCollection.Find(_ => true).ToListAsync();
            }

            return new List<Instance>();
        }
    }
}
