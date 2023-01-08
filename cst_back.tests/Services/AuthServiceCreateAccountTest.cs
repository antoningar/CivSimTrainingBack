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
    public class AuthServiceCreateAccountTest
    {
        [Theory]
        [InlineData("aa", "a@a.com", "aaaaaaaa", "aaaaaaaa")]
        [InlineData("aaaa", "a.a.com", "aaaaaaaa", "aaaaaaaa")]
        [InlineData("aaaa", "a@a.com", "aaaa", "aaaa")]
        [InlineData("aaaa", "a@a.com", "aaaaaaaa", "aaaa")]
        public async Task CreateAccount_ShouldTestInputDatas(string username, string email, string password, string confPassword)
        {
            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));
            Mock<IAccountDBService> dbAccountServiceMock = new();
            Mock<ICounterDBService> dbCounterServices = new();
            Mock<ICryptoHelper> cryptoHelper = new();

            Auth.AuthBase authService = new AuthService(new CreateAccountValidator(), new ConnectValidator(), dbAccountServiceMock.Object, cryptoHelper.Object);

            CreateAccountRequest createRequest = new()
            {
                Username = username,
                Email = email,
                Password = password,
                ConfPassword = confPassword
            };

            await Assert.ThrowsAsync<RpcException>(() => authService.CreateAccount(createRequest, context));
        }

        [Theory]
        [InlineData("sil2ob", "a@a.com", "aaaaaaaa", "aaaaaaaa")]
        [InlineData("aaaa", "sil2ob@a.com", "aaaaaaaa", "aaaaaaaa")]
        public async Task CreateAccount_ShouldCheckIfAnAccountAlreadytExist(string username, string email, string password, string confPassword)
        {
            Account account = new()
            {
                Email = email,
                Username = username
            };

            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));

            Mock<IAccountDBService> dbServiceMock = new();
            dbServiceMock
                .Setup(x => x.GetAccountByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(account);
            dbServiceMock
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(account);
            Mock<ICryptoHelper> cryptoHelper = new();

            Auth.AuthBase authService = new AuthService(new CreateAccountValidator(), new ConnectValidator(), dbServiceMock.Object, cryptoHelper.Object);

            CreateAccountRequest createRequest = new()
            {
                Username = username,
                Email = email,
                Password = password,
                ConfPassword = confPassword
            };

            await Assert.ThrowsAsync<RpcException>(() => authService.CreateAccount(createRequest, context));
        }

        [Theory]
        [InlineData("sil2ob", "a@a.com", "aaaaaaaa", "aaaaaaaa")]
        public async Task CreateAccount_ShouldCreateAccount(string username, string email, string password, string confPassword)
        {
            int counter = 5;

            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));

            Mock<IAccountDBService> dbAccountServiceMock = new();
            dbAccountServiceMock
                .Setup(x => x.InsertAccountAsync(It.IsAny<Account>()))
                .ReturnsAsync(counter + 1);
            Mock<ICryptoHelper> cryptoHelper = new();

            Auth.AuthBase authService = new AuthService(new CreateAccountValidator(), new ConnectValidator(), dbAccountServiceMock.Object, cryptoHelper.Object);

            CreateAccountRequest createRequest = new()
            {
                Username = username,
                Email = email,
                Password = password,
                ConfPassword = confPassword
            };

            CreateAccountResponse response = await authService.CreateAccount(createRequest, context);
            Assert.Equal(counter + 1, response.Id);
            dbAccountServiceMock.Verify(s => s.InsertAccountAsync(It.IsAny<Account>()), Times.Once());
        }
    }
}
