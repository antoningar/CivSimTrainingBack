using cst_back.DBServices;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using cst_back.specs.Fixtures;
using cst_back.Validators;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Moq;

namespace cst_back.specs.StepDefinitions.Instances
{
    [Binding]
    public class GetInstancesStepDefinitions
    {
        private readonly InstancesRequest _instancesRequest = new();
        private List<InstancesResponse> _response = new();
        private TestServer? _instancesServer;
        private RPCInstance.RPCInstanceClient? _instancesClient;

        [Scope(Feature = "GetInstances")]
        [Given(@"I am a user")]
        public void GivenIAmAUser()
        {
            GetInstancesValidator getInstancesValidator = new();

            Mock<IInstanceDBService> dbServiceMock = new();
            dbServiceMock
                .Setup(x => x.GetInstances(It.IsAny<string>()))
                .ReturnsAsync(new List<Instance>() {
                    new Instance() {
                        Civilization= "Civilization",
                        Goal="Goal",
                        Map="Map"
                    }
                });

            RPCInstance.RPCInstanceBase instancesService = new InstanceService(getInstancesValidator, dbServiceMock.Object);
            _instancesServer = ServersFixtures.GetInstancesServer(instancesService, dbServiceMock);
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
            {
                HttpClient = _instancesServer.CreateClient()
            });
            _instancesClient = new RPCInstance.RPCInstanceClient(channel);
        }

        [When(@"I want all instances")]
        public async Task WhenIWantAllInstances()
        {
            using var call = _instancesClient!.GetInstances(_instancesRequest);
            await foreach (InstancesResponse response in call.ResponseStream.ReadAllAsync())
            {
                _response.Add(response);
            }
        }

        [Then(@"I get a list of instances")]
        public void ThenIGetAListOfInstances()
        {
            _response.Should().NotBeNullOrEmpty();
        }
    }
}