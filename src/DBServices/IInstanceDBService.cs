using cst_back.Models;
using cst_back.Protos;

namespace cst_back.DBServices
{
    public interface IInstanceDBService
    {
        public Task<List<Instance>> GetInstances(Filter filter);
    }
}
