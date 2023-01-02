using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Services;
using cst_back.specs.Fixtures;
using cst_back.Validators;
using Grpc.Net.Client;
using MongoDB.Driver;
using Moq;

namespace cst_back.specs.StepDefinitions.Connect
{
    [Binding]
    public class ConnectStepDefinitions
    {
        private readonly ConnectRequest _connectRequest = new();
        private ConnectResponse? _connectResponse;
        private Microsoft.AspNetCore.TestHost.TestServer? _connectServer;
        private Auth.AuthClient? _connectClient;

        [Scope(Feature = "Connect")]
        [Given(@"I am a user")]
        public void GivenIAmAUser()
        {
            CreateAccountValidator createValidator = new();
            ConnectValidator connectValidator = new();

            var collectionMock = Mock.Of<IMongoCollection<Account>>();

            Mock<IAccountDBService> dbServiceMock = new();
            dbServiceMock
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(new Account()
                {
                    AccountId = 1,
                });
            Mock<ICryptoHelper> cryptoHelper = new();
            cryptoHelper
                .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            Auth.AuthBase authService = new AuthService(createValidator, connectValidator, dbServiceMock.Object, cryptoHelper.Object);
            _connectServer = ServersFixtures.GetAuthServer(authService, dbServiceMock);
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
            {
                HttpClient = _connectServer.CreateClient()
            });
            _connectClient = new Auth.AuthClient(channel);
        }

        [Given(@"My ids are ""([^""]*)"" / ""([^""]*)""")]
        public void GivenMyIdsAre(string username, string password)
        {
            _connectRequest.Username = username;
            _connectRequest.Password = password;
        }

        [When(@"I login")]
        public void WhenILogin()
        {
            _connectResponse = _connectClient!.Connect(_connectRequest);
        }

        [Then(@"I have access to my account")]
        public void ThenIHaveAccessToMyAccount()
        {
            Assert.True(_connectResponse!.Id > 0);
        }
    }
}
