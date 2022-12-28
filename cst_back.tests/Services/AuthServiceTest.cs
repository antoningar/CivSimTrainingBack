using Castle.Core.Logging;
using cst_back.Services;
using cst_back.Validators;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;

namespace cst_back.tests.Services
{
    public class AuthServiceTest
    {
        [Theory]
        [InlineData("aa","a@a.com","aaaaaaaa","aaaaaaaa")]
        [InlineData("aaaa", "a.a.com", "aaaaaaaa", "aaaaaaaa")]
        [InlineData("aaaa", "a@a.com", "aaaa", "aaaa")]
        [InlineData("aaaa", "a@a.com", "aaaaaaaa", "aaaa")]
        public async Task CreateAccount_ShouldTestInputDatas(string username, string email, string password, string confPassword)
        {
            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));
            Mock<ILogger<AuthService>> mockLogger = new();
            AuthService authService = new(mockLogger.Object, new CreateAccountValidator());

            CreateAccountRequest createRequest = new()
            {
                Username = username,
                Email = email,
                Password = password,
                ConfPassword = confPassword
            };

            await Assert.ThrowsAsync<RpcException>(() => authService.CreateAccount(createRequest, context));
        }
    }
}
