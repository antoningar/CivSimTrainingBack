using cst_back.Protos;
using Grpc.Core;

namespace cst_back.Services
{
    public interface IInstanceService
    {
        public Task GetInstances(InstancesRequest request, IServerStreamWriter<InstancesResponse> responseStream, ServerCallContext context);
        public Task SearchInstances(SearchInstancesRequest request, IServerStreamWriter<InstancesResponse> responseStream, ServerCallContext context);
        public Task<InstancesDetailsResponse> GetInstancesDetails(InstancesDetailsRequest request, ServerCallContext context);

        public Task<CreateInstanceResponse> CreateInstance(CreateInstanceRequest request, ServerCallContext context);
    }
}
