using cst_back.Models;

namespace cst_back.DBServices
{
    public interface IAccountDBService
    {
        public Task<Account?> GetAccountByUsernameAsync(string username);
        public Task<Account?> GetAccountByEmailAsync(string email);
        public Task<int?> InsertAccountAsync(Account account);
    }
}
