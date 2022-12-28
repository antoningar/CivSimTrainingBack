using Castle.Core.Logging;
using cst_back.Models;
using cst_back.Services;
using cst_back.Validators;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;

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
            Mock<IAccountDBService> dbServiceMock = new();
            Auth.AuthBase authService = new AuthService(mockLogger.Object, new CreateAccountValidator(), dbServiceMock.Object);

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
            Account account = new Account()
            {
                Email= email,
                Username=username
            };

            var context = Helper.GetServerCallContext(nameof(IAuthService.CreateAccount));
            Mock<ILogger<AuthService>> mockLogger = new();

            Mock<IAccountDBService> dbServiceMock = new();
            dbServiceMock
                .Setup(x => x.GetAccountByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(account);
            dbServiceMock
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(account);
            Auth.AuthBase authService = new AuthService(mockLogger.Object, new CreateAccountValidator(), dbServiceMock.Object);

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
