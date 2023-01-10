using cst_back.DBServices;
using cst_back.Protos;
using cst_back.Services;
using cst_back.specs.Fixtures;
using cst_back.tests;
using cst_back.Validators;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Moq;
using System;
using TechTalk.SpecFlow;
using ZstdSharp.Unsafe;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    public class GetInstanceDetailsStepDefinitions
    {
        private InstancesDetailsRequest _request = new();
        private InstancesDetailsResponse? _response;
        private TestServer? _instancesServer;
        private RPCInstance.RPCInstanceClient? _instancesClient;

        [Scope(Feature = "GetInstanceDetails")]
        [Given(@"I am a user")]
        public void GivenIAmAUser()
        {
            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            mockLeaderboardDBService
                .Setup(x => x.GetLeaderboard(It.IsAny<string>()))
                .ReturnsAsync(Helper.GetLeaderBoard());
            Mock<IInstanceDBService> dbServiceMock = new();
            dbServiceMock
                .Setup(x => x.GetInstance(It.IsAny<string>()))
                .ReturnsAsync(Helper.GetInstance());

            RPCInstance.RPCInstanceBase instancesService = new InstanceService(dbServiceMock.Object, mockLeaderboardDBService.Object);
            _instancesServer = ServersFixtures.GetInstancesServer(instancesService, dbServiceMock, mockLeaderboardDBService);
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
            {
                HttpClient = _instancesServer.CreateClient()
            });
            _instancesClient = new RPCInstance.RPCInstanceClient(channel);
        }

        [When(@"I got an instance with id ""([^""]*)""")]
        public void WhenIGotAnInstanceWithId(string id)
        {
            _request.Id = id;
        }


        [When(@"I need his details")]
        public void WhenINeedHisDetails()
        {
            _response = _instancesClient!.GetInstancesDetails(_request);
        }

        [Then(@"I got details")]
        public void ThenIGotDetails()
        {
            Assert.NotEmpty(_response!.Id);
            Assert.NotEmpty(_response!.Civilization);
            Assert.NotEmpty(_response!.Map);
            Assert.NotEmpty(_response!.Goal);
            Assert.NotEmpty(_response!.Leaderboard!.Results);
        }
    }
}
