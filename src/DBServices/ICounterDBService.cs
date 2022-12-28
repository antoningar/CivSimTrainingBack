namespace cst_back.DBServices
{
    public interface ICounterDBService
    {
        public Task<int?> GetUserIdCounterAsync();
        public Task IncrementUserIdCounterAsync();
    }
}
