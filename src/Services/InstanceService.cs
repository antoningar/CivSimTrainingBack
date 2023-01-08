using cst_back.DBServices;
using cst_back.Models;
using cst_back.Protos;
using FluentValidation;
using Grpc.Core;
using MongoDB.Driver;

namespace cst_back.Services
{
    public class InstanceService : RPCInstance.RPCInstanceBase, IInstanceService
    {
        private readonly IValidator<InstancesRequest> _instanceRequestValidator;
        private readonly IInstanceDBService _instanceDBService;

        public InstanceService(IValidator<InstancesRequest> instanceRequestValidator, IInstanceDBService instanceDBService)
        {
            _instanceRequestValidator = instanceRequestValidator;
            _instanceDBService = instanceDBService;
        }

        private void CheckGetInstanceRequest(InstancesRequest request)
        {
            var validate = _instanceRequestValidator.Validate(request);
            if (!validate.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Filter not found"));
            }
        }

        private async Task writeInstancesToStream(List<Instance> instances, IServerStreamWriter<InstancesResponse> strean)
        {
            foreach (Instance instance in instances)
            {
                await strean.WriteAsync(new InstancesResponse()
                {
                    Civilization = instance.Civilization,
                    Goal = instance.Goal,
                    Map = instance.Map,
                });
            }
        }

        public override async Task GetInstances(InstancesRequest request, IServerStreamWriter<InstancesResponse> responseStream, ServerCallContext context)
        {
            List<Instance> instances = new();
            CheckGetInstanceRequest(request);
            try
            {
                instances = await _instanceDBService.GetInstances(request.Filter);
            }
            catch (MongoException ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            await writeInstancesToStream(instances, responseStream);
        }
    }
}
