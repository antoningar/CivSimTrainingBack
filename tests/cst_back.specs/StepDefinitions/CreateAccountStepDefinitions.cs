using cst_back.Models;
using cst_back.Services;
using cst_back.specs.Fixtures;
using cst_back.Validators;
using FluentValidation;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    public class CreateAccountStepDefinitions
    {
        private readonly CreateAccountRequest _createRequest = new();
        private CreateAccountResponse? _createResponse;
        private TestServer? _authServer;
        private Auth.AuthClient? _authClient;

        [Given(@"As a user")]
        public void GivenAsAUser()
        {
            Mock<ILogger<AuthService>> mockLogger = new();
            CreateAccountValidator validator = new();


            var collectionMock = Mock.Of<IMongoCollection<Account>>();
            Mock<IMongoDatabase> dbMock = new();
            dbMock
                .Setup(x => x.GetCollection<Account>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(collectionMock);

            Mock<IAccountDBService> dbServiceMock = new();
            dbServiceMock
                .Setup(x => x.GetAccountByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Account>());
            dbServiceMock
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Account>());

            Auth.AuthBase authService = new AuthService(mockLogger.Object, validator, dbServiceMock.Object);
            _authServer = ServersFixtures.GetAuthServer(authService);
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
            {
                HttpClient = _authServer.CreateClient()
            });
            _authClient = new Auth.AuthClient(channel);
        }

        [Given(@"My username is ""([^""]*)""")]
        public void GivenMyUsernameIs(string username)
        {
            _createRequest.Username = username;
        }

        [Given(@"My email  is ""([^""]*)""")]
        public void GivenMyEmailIs(string email)
        {
            _createRequest.Email = email;
        }

        [Given(@"My password is ""([^""]*)""")]
        public void GivenMyPasswordIs(string password)
        {
            _createRequest.Password = password;
        }

        [Given(@"My password confirmation is ""([^""]*)""")]
        public void GivenMyPasswordConfirmationIs(string confPassword)
        {
            _createRequest.ConfPassword = confPassword;
        }

        [When(@"I want to create my  account")]
        public void WhenIWantToCreateMyAccount()
        {
            _createResponse = _authClient!.CreateAccount(_createRequest);
        }

        [Then(@"I should receive a response my id")]
        public void ThenIShouldReceiveAResponseMyId()
        {
            _authServer!.Dispose();
            Assert.True(Convert.ToInt64(_createResponse!.Id) > 0);
        }

        [When(@"I want to create my  account, I shouldn't received my id")]
        public void WhenIWantToCreateMyAccountIShouldntReceivedMyId()
        {
            Assert.ThrowsAny<RpcException>(() => _authClient!.CreateAccount(_createRequest));
        }
    }
}
