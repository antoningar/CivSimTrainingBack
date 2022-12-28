using cst_back.Models;

namespace cst_back.Services
{
    public interface IAccountDBService
    {
        public Task<Account?> GetAccountByUsernameAsync(string username);
        public Task<Account?> GetAccountByEmailAsync(string email);
        public Task<List<Account>> InsertAccountAsync(Account account);
    }
}
