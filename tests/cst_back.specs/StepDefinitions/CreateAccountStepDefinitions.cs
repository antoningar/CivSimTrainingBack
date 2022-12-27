using cst_back.Services;
using cst_back.specs.Fixtures;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    public class CreateAccountStepDefinitions
    {
        private readonly CreateRequest _createRequest = new();
        private CreateResponse? _createResponse;
        private TestServer? _authServer;
        private Auth.AuthClient? _authClient;

        [Given(@"As a user")]
        public void GivenAsAUser()
        {
            Mock<ILogger<AuthService>> mockLogger = new();
            Auth.AuthBase authService = new AuthService(mockLogger.Object);
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

        [Then(@"I should receive a response  (.*)")]
        public void ThenIShouldReceiveAResponse(int p0)
        {
            _authServer!.Dispose();
            Assert.True(Convert.ToInt64(_createResponse!.Id) > 0);
        }
    }
}
