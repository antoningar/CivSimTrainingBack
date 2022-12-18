using cst_back.Services;
using cst_back.specs.Fixtures;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    public class CreateAccountStepDefinitions
    {
        private HttpClient _httpClient;
        private readonly CreateRequest _createRequest = new();
        private CreateResponse _createResponse;

        [Given(@"As a user")]
        public void GivenAsAUser()
        {
            Mock<ILogger<AuthService>> mockLogger = new();
            IAuthService authService = new AuthService(mockLogger.Object);
            TestServer _authServer = ServersFixtures.GetAuthServer(authService);
            _httpClient = _authServer.CreateClient();

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
        public async Task WhenIWantToCreateMyAccount()
        {
            string jsonRequest = JsonConvert.SerializeObject(_createRequest);
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("Auth/CreateAccount", UriKind.Relative),
                Content = new StringContent(jsonRequest)
            };
            HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);
            var strReponse = await response.Content.ReadAsStringAsync();
            _createResponse = JsonConvert.DeserializeObject<CreateResponse>(strReponse);
        }

        [Then(@"I should receive a response  (.*)")]
        public void ThenIShouldReceiveAResponse(int p0)
        {
            Assert.True(Convert.ToInt64(_createResponse.Id) > 0);
        }
    }
}
