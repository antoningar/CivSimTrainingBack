using cst_back.DBServices;
using cst_back.Helpers;
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
        private readonly IValidator<string> _goalValidator;

        private readonly IInstanceDBService _instanceDBService;
        private readonly ILeaderboardDBService _leaderboardDBService;
        private readonly IAccountDBService _accountDBService;
        private readonly IFileHelper _fileHelper;
        private readonly IFileDBService _fileDBService;

        public InstanceService(IInstanceDBService instanceDBService, ILeaderboardDBService leaderboardDBService, IAccountDBService accountDBService, IFileHelper fileHelper, IFileDBService fileDBService)
        {
            _instanceRequestValidator = new GetInstancesValidator();
            _searchInstanceRequestValidator = new SearchInstancesValidator();
            _goalValidator = new GoalValidator();
            _instanceDBService = instanceDBService;
            _leaderboardDBService = leaderboardDBService;
            _accountDBService = accountDBService;
            _fileHelper = fileHelper;
            _fileDBService = fileDBService;
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

        private async Task CheckCreateInstancePreconditions(CreateInstanceRequest request)
        {
            Account? account = null;
            try
            {
                account = await _accountDBService.GetAccountByUsernameAsync(request.Username);
            }
            catch (MongoException ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            if (account == null)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Account not found"));
            }

            var goalValidation = _goalValidator.Validate(request.Goal);
            if (!goalValidation.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, goalValidation.Errors.First().ErrorMessage));
            }

            if (!_fileHelper.IsInstanceTmpFilesExist(request.Username))
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Saves not found"));
        }

        private async Task<Instance?> InsertInstance(CreateInstanceRequest request)
        {
            try
            {
                Instance? instance = await _fileHelper.GetInstanceFromFile(request.Username);
                Account? account = await _accountDBService.GetAccountByUsernameAsync(request.Username);
                instance.Creator = account!.Username;
                instance.Goal = request.Goal;
                return await _instanceDBService.InsertInstance(instance);
            }
            catch (MongoException ex)
            {
                throw new RpcException(new Status(StatusCode.Internal,ex.Message));
            }
            catch(Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private async Task InsertFile(string userId, string instanceId)
        {
            try
            {
                await _fileDBService.SaveFile(userId, instanceId);
            }
            catch (MongoException ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private string GetValueFromGoal(string goal, Stats stats)
        {
            switch (goal)
            {
                default:
                    return stats.Gold.ToString();
            }
        }

        private async Task CreateLeaderboard(Instance instance)
        {
            Stats stats = await _fileHelper.GetGameStatsFromfile(instance!.Id!);
            Models.Leaderboard leaderboard = new()
            {
                InstanceId = instance.Id,
                Results = new Models.Result[]
                {
                    new Models.Result
                    {
                        Position = 1,
                        Username = instance.Creator,
                        Value = GetValueFromGoal(instance.Goal!, stats)
                    }
                }
            };
            await _leaderboardDBService.InsertLeaderboard(leaderboard);
        }

        public override async Task<CreateInstanceResponse> CreateInstance(CreateInstanceRequest request, ServerCallContext context)
        {
            await CheckCreateInstancePreconditions(request);
            Instance? instance = await InsertInstance(request);
            await InsertFile(request.Username, instance!.Id!);
            await CreateLeaderboard(instance!);

            _fileHelper.DeleteTmpFileByUsername(request.Username);

            return new CreateInstanceResponse
            {
                Id = instance!.Id!
            };
        }
    }
}
