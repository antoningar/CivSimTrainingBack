using cst_back.Models;

namespace cst_back.DBServices
{
    public interface IInstanceDBService
    {
        public Task<List<Instance>> GetInstances(string filter);
    }
}
