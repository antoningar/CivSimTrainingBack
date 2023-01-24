namespace cst_back.DBServices
{
    public interface IFileDBService
    {
        public Task SaveFile(string userId, string instanceId);
    }
}
