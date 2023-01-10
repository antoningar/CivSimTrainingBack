using cst_back.DBServices;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Validators;
using FluentValidation;
using Grpc.Core;
using MongoDB.Driver;

namespace cst_back.Services
{
    public class InstanceService : RPCInstance.RPCInstanceBase, IInstanceService
    {
        private readonly IValidator<InstancesRequest> _instanceRequestValidator;
        private readonly IValidator<SearchInstancesRequest> _searchInstanceRequestValidator;

        private readonly IInstanceDBService _instanceDBService;
        private readonly ILeaderboardDBService _leaderboardDBService;

        public InstanceService(IInstanceDBService instanceDBService, ILeaderboardDBService leaderboardDBService)
        {
            _instanceRequestValidator = new GetInstancesValidator();
            _searchInstanceRequestValidator = new SearchInstancesValidator();
            _instanceDBService = instanceDBService;
            _leaderboardDBService = leaderboardDBService;
        }

        private void CheckRequest<T>(IValidator<T> validator, T request)
        {
            var validate = validator.Validate(request);
            if (!validate.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, validate.Errors[0].ErrorMessage));
            }
        }


        private void CheckGetInstanceRequest(InstancesRequest request)
        {
            var validate = _instanceRequestValidator.Validate(request);
            if (!validate.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, validate.Errors[0].ErrorMessage));
            }
        }

        private async Task writeInstancesToStream(List<Instance> instances, IServerStreamWriter<InstancesResponse> strean)
        {
            foreach (Instance instance in instances)
            {
                await strean.WriteAsync(new InstancesResponse()
                {
                    Id = instance.Id,
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

        private void CheckSearchInstancesRequest(SearchInstancesRequest request)
        {
            var validate = _searchInstanceRequestValidator.Validate(request);
            if (!validate.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, validate.Errors[0].ErrorMessage));
            }
        }

        public override async Task SearchInstances(SearchInstancesRequest request, IServerStreamWriter<InstancesResponse> responseStream, ServerCallContext context)
        {
            CheckSearchInstancesRequest(request);
            try
            {
                List<Instance> instances = await _instanceDBService.SearchInstances(request.Search);
                await writeInstancesToStream(instances, responseStream);
            }
            catch (MongoException ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private async Task<Instance> GetInstance(string id)
        {
            Instance? instance = await _instanceDBService.GetInstance(id);
            if (instance == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Instance not found"));
            }
            return instance;
        }

        public override async Task<InstancesDetailsResponse> GetInstancesDetails(InstancesDetailsRequest request, ServerCallContext context)
        {
            try
            {
                Instance instance = await GetInstance(request.Id);
                Models.Leaderboard? leaderboard = await _leaderboardDBService.GetLeaderboard(request.Id);
                InstanceDetails response = new(instance, leaderboard);
                return response.InstancesDetailsResponse();
            }
            catch (MongoException ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}
