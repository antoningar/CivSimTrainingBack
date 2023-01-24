using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Protos;
using cst_back.specs.Fixtures;
using cst_back.tests;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Moq;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    [Scope(Feature = "CreateInstance")]
    public class CreateInstanceStepDefinitions
    {
        private CreateInstanceRequest _request = new();
        private CreateInstanceResponse? _response;
        private TestServer? _server;
        private RPCInstance.RPCInstanceClient? _client;

        [Given(@"I am a user")]
        public void GivenIAmAUser()
        {
            Mock<IAccountDBService> mockAccountDBService = new();
            mockAccountDBService
                .Setup(x => x.GetAccountByUserId(It.IsAny<string>()))
                .ReturnsAsync(new Models.Account() { Id = "123", Username  = "aaa" });

            Mock<IFileHelper> mockFileHelper= new();
            mockFileHelper
                .Setup(x => x.IsInstanceTmpFilesExist(It.IsAny<string>()))
                .Returns(true);
            mockFileHelper
                .Setup(x => x.GetInstanceFromFile(It.IsAny<string>()))
                .ReturnsAsync(Helper.GetInstance());

            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.InsertInstance(It.IsAny<Instance>()))
                .ReturnsAsync("12");

            RPCInstance.RPCInstanceBase instancesService = Helper.GetInstanceService(mockInstanceDBService: mockInstanceDBService);
            _server = ServersFixtures.GetInstancesServer(instanceService: instancesService, mockAccountDBService: mockAccountDBService, mockFileHelper: mockFileHelper, mockInstanceDBService: mockInstanceDBService);
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
            {
                HttpClient = _server.CreateClient()
            });
            _client = new RPCInstance.RPCInstanceClient(channel);
        }

        [Given(@"my userid is ""([^""]*)""")]
        public void GivenMyUseridIs(string id)
        {
            _request.UserId = id;
        }

        [Given(@"my goal is ""([^""]*)""")]
        public void GivenMyGoalIs(string goal)
        {
            _request.Goal = goal;
        }

        [When(@"I want t create my instances")]
        public void WhenIWantTCreateMyInstances()
        {
            _response = _client!.CreateInstance(_request);
        }

        [Then(@"I receive my instance id")]
        public void ThenIReceiveMyInstanceId()
        {
            Assert.True(!string.IsNullOrWhiteSpace(_response!.Id));
        }
    }
}
