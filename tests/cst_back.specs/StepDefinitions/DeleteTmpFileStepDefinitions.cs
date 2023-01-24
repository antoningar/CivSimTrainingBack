using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Protos;
using cst_back.specs.Fixtures;
using cst_back.tests;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Moq;
using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Analytics.UserId;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    [Scope(Feature = "DeleteTmpFile")]
    public class DeleteTmpFileStepDefinitions
    {
        private DeleteTmpFilesRequest _request = new DeleteTmpFilesRequest();
        private DeleteTmpFilesResponse? _response =  null;
        private TestServer? _server;
        private RPCFileInfo.RPCFileInfoClient? _client;

        [Given(@"I am a client")]
        public void GivenIAmAClient()
        {
            Mock<IFileHelper> mockFileHelper = new();
            mockFileHelper
                .Setup(x => x.DeleteTmpFileByUsername(It.IsAny<string>()))
                .Returns(1);
            Mock<IAccountDBService> mockAccountDBService = new();
            mockAccountDBService
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(new Account() { Username = "bahtiens" });

            _server = ServersFixtures.GetFileInfoServer(mockFileHelper, mockAccountDBService);
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
            {
                HttpClient = _server.CreateClient()
            });
            _client = new RPCFileInfo.RPCFileInfoClient(channel);
        }

        [Given(@"My username is ""([^""]*)""")]
        public void GivenMyUsernameIs(string username)
        {
            _request.Username = username;
        }

        [When(@"I want to delete my tmp files")]
        public void WhenIWantToDeleteMyTmpFiles()
        {
            _response  = _client!.DeleteTmpFiles(_request);
        }

        [Then(@"My files are deleted")]
        public void ThenMyFilesAreDeleted()
        {
            Assert.True(_response!.NumberDeleted > 0);
        }
    }
}
