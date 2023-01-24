using cst_back.DBServices;
using cst_back.Protos;
using cst_back.Services;
using cst_back.specs.Fixtures;
using cst_back.tests;
using cst_back.Validators;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Moq;
using System;
using TechTalk.SpecFlow;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    public class SearchInstancesStepDefinitions
    {
        private List<InstancesResponse> _response = new();
        private TestServer? _instancesServer;
        private RPCInstance.RPCInstanceClient? _client;
        private SearchInstancesRequest _request = new();

        [Scope(Feature ="SearchInstances")]
        [Given(@"I am a user")]
        public void GivenIAmAUser()
        {
        }

        [Given(@"I searching instances with ""([^""]*)""")]
        public void GivenISearchingInstancesWith(string search)
        {
            _request.Search = search;

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> dbServiceMock = new();
            Mock<IAccountDBService> mockAccountDbService = new();
            dbServiceMock
                .Setup(x => x.SearchInstances(It.IsAny<string>()))
                .ReturnsAsync((string s) => Helper.GetListInstanceBySearch(s));

            _instancesServer = ServersFixtures.GetInstancesServer(dbServiceMock, mockLeaderboardDBService);
            var channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                HttpClient = _instancesServer.CreateClient()
            });
            _client = new RPCInstance.RPCInstanceClient(channel);
        }

        [Scope(Feature = "SearchInstances")]
        [When(@"I want all instances")]
        public async Task WhenIWantAllInstances()
        {
            using var call = _client!.SearchInstances(_request);
            await foreach (InstancesResponse response in call.ResponseStream.ReadAllAsync())
            {
                _response.Add(response);
            }
        }

        [Then(@"They all contains ""([^""]*)""")]
        public void ThenTheyAllContains(string victory)
        {
            foreach (var instance in _response)
            {
                Assert.True(Helper.InstancesResponseContainsSearch(instance, _request.Search));
            }
        }
    }
}
