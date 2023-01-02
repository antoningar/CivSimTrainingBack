using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Services;
using cst_back.specs.Fixtures;
using cst_back.Validators;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Moq;

namespace cst_back.specs.StepDefinitions.Create
{
    [Binding]
    public class CreateAccountStepDefinitions
    {
        private readonly CreateAccountRequest _createRequest = new();
        private CreateAccountResponse? _createResponse;
        private TestServer? _authServer;
        private Auth.AuthClient? _authClient;

        [Scope(Feature = "CreateAccountError")]
        [Scope(Feature = "CreateAccount")]
        [Given(@"I am a user")]
        public void GivenIAmAUser()
        {
            CreateAccountValidator createValidator = new();
            ConnectValidator connectValidator = new();

            Mock<IAccountDBService> dbServiceMock = new();
            dbServiceMock
                .Setup(x => x.InsertAccountAsync(It.IsAny<Account>()))
                .ReturnsAsync(1);
            Mock<ICryptoHelper> cryptoHelper = new();

            Auth.AuthBase authService = new AuthService(createValidator, connectValidator, dbServiceMock.Object, cryptoHelper.Object);
            _authServer = ServersFixtures.GetAuthServer(authService, dbServiceMock);
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
            _authServer!.Dispose();
            Assert.ThrowsAny<RpcException>(() => _authClient!.CreateAccount(_createRequest));
        }
    }
}
