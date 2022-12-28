using cst_back.Models;
using cst_back.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace cst_back.Services
{
    public class AccountDBService : IAccountDBService
    {
        private readonly IMongoCollection<Account> _accountCollection;

        public AccountDBService(IOptions<AccountDatabaseSettings> accountSettings)
        {
            MongoClient mongoClient = new(accountSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(accountSettings.Value.DatabaseName);
            _accountCollection = mongoDatabase.GetCollection<Account>(
                accountSettings.Value.AccountCollectionName);
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _accountCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            return await _accountCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<List<Account>> InsertAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
