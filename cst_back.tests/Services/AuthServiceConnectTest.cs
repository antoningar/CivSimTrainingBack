using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Services;
using cst_back.Validators;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace cst_back.tests.Services
{
    public class AuthServiceConnectTest
    {
        [Theory]
        [InlineData("aa","aaaaaaaa")]
        [InlineData("aaaa", "aaa")]
        public async Task CreateAccount_ShouldTestInputDatas(string username, string password)
        {
            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));
            Mock<ILogger<AuthService>> mockLogger = new();
            Mock<IAccountDBService> dbAccountServiceMock = new();
            Mock<ICryptoHelper> cryptoHelper = new();

            Auth.AuthBase authService = new AuthService(mockLogger.Object, new CreateAccountValidator(), new ConnectValidator(), dbAccountServiceMock.Object, cryptoHelper.Object);

            ConnectRequest connectRequest = new()
            {
                Username = username,
                Password = password
            };

            await Assert.ThrowsAsync<RpcException>(() => authService.Connect(connectRequest, context));
        }

        [Theory]
        [InlineData("jean", "jeancharles")]
        public async Task CreateAccount_ShouldTestIfAccountDoesntExist(string username, string password)
        {
            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));
            Mock<ILogger<AuthService>> mockLogger = new();
            Mock<IAccountDBService> dbAccountServiceMock = new();
            dbAccountServiceMock
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(null as Account);
            Mock<ICryptoHelper> cryptoHelper = new();

            Auth.AuthBase authService = new AuthService(mockLogger.Object, new CreateAccountValidator(), new ConnectValidator(), dbAccountServiceMock.Object, cryptoHelper.Object);

            ConnectRequest connectRequest = new()
            {
                Username = username,
                Password = password
            };

            await Assert.ThrowsAsync<RpcException>(() => authService.Connect(connectRequest, context));
            dbAccountServiceMock.Verify(x => x.GetAccountByUsernameAsync(It.IsAny<string>()), Times.Once());
        }

        [Theory]
        [InlineData("jean", "jeancharles")]
        public async Task CreateAccount_ShouldReturnId(string username, string password)
        {
            Account account = new()
            {
                AccountId = 4
            };
            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));
            Mock<ILogger<AuthService>> mockLogger = new();
            Mock<IAccountDBService> dbAccountServiceMock = new();
            dbAccountServiceMock
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(account);
            Mock<ICryptoHelper> cryptoHelper = new();
            cryptoHelper
                .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            Auth.AuthBase authService = new AuthService(mockLogger.Object, new CreateAccountValidator(), new ConnectValidator(), dbAccountServiceMock.Object, cryptoHelper.Object);

            ConnectRequest connectRequest = new()
            {
                Username = username,
                Password = password
            };

            ConnectResponse response = await authService.Connect(connectRequest, context);

            Assert.Equal(account.AccountId, response.Id);
            dbAccountServiceMock.Verify(x => x.GetAccountByUsernameAsync(It.IsAny<string>()), Times.Once());
        }
    }
}
