using cst_back.Models;
using cst_back.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace cst_back.DBServices
{
    public class AccountDBService : IAccountDBService
    {
        private readonly ICounterDBService _counterDBService;
        private readonly IMongoCollection<Account> _accountCollection;

        public AccountDBService(IOptions<DatabaseSettings> dbSettings, ICounterDBService counterDBService)
        {
            MongoClient mongoClient = new(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _accountCollection = mongoDatabase.GetCollection<Account>(
                dbSettings.Value.AccountCollectionName);

            _counterDBService = counterDBService;
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _accountCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            return await _accountCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<int?> InsertAccountAsync(Account account)
        {
            int? counter = await _counterDBService.GetUserIdCounterAsync();
            account.AccountId = ++counter;
            await _accountCollection.InsertOneAsync(account);
            await _counterDBService.IncrementUserIdCounterAsync();

            return account.AccountId;
        }

        public async Task<Account?> GetAccountByUsernameAndHashedPassword(string username, string password)
        {
            return await _accountCollection.Find(x => x.Username == username && BCrypt.Net.BCrypt.Verify(x.Password, password, false, BCrypt.Net.HashType.SHA384) == true).FirstOrDefaultAsync();
        }
    }
}
