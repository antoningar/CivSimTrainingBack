using cst_back.Models;
using cst_back.Protos;

namespace cst_back.DBServices
{
    public interface IInstanceDBService
    {
        public Task<List<Instance>> GetInstances(Filter filter);
        public Task<List<Instance>> SearchInstances(string search);
        public Task<Instance?> GetInstance(string id);
        public Task<Instance?> InsertInstance(Instance instance);
    }
}
