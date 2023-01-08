using cst_back.DBServices;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using cst_back.specs.Fixtures;
using cst_back.tests;
using cst_back.Validators;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Moq;
using System.Reflection;

namespace cst_back.specs.StepDefinitions.Instances
{
    [Binding]
    public class GetInstancesStepDefinitions
    {
        private readonly InstancesRequest _instancesRequest = new()
        {
            Filter = new()
            {
                Type = ""
            }
        };
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
                .Setup(x => x.GetInstances(It.Is<Filter>(x => string.IsNullOrWhiteSpace(x.Type))))
                .ReturnsAsync(Helper.GetListInstance());
            
            dbServiceMock
                .Setup(x => x.GetInstances(new Filter() { Type= "Civilization", Value="Dido" }))
                .ReturnsAsync(Helper.GetListInstance().FindAll(x => x.Civilization == "Dido"));
            dbServiceMock
                .Setup(x => x.GetInstances(new Filter() { Type = "Goal", Value = "Scientific Victory" }))
                .ReturnsAsync(Helper.GetListInstance().FindAll(x => x.Goal == "Scientific Victory"));
            dbServiceMock
                .Setup(x => x.GetInstances(new Filter() { Type = "Map", Value = "Seven Seas" }))
                .ReturnsAsync(Helper.GetListInstance().FindAll(x => x.Map == "Seven Seas"));


            RPCInstance.RPCInstanceBase instancesService = new InstanceService(getInstancesValidator, dbServiceMock.Object);
            _instancesServer = ServersFixtures.GetInstancesServer(instancesService, dbServiceMock);
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
            {
                HttpClient = _instancesServer.CreateClient()
            });
            _instancesClient = new RPCInstance.RPCInstanceClient(channel);
        }

        [Given(@"I'm focus on ""([^""]*)"" ""([^""]*)""")]
        public void GivenImFocusOn(string type, string value)
        {
            _instancesRequest.Filter.Type = type;
            _instancesRequest.Filter.Value = value;
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

        [Then(@"Their ""([^""]*)"" are only ""([^""]*)""")]
        public void ThenTheirAreOnly(string type, string value)
        {
            foreach (var response in _response)
            {
                PropertyInfo? property = response.GetType().GetProperty(type);
                Assert.Equal(value, property!.GetValue(response));
            }
        }
    }
}