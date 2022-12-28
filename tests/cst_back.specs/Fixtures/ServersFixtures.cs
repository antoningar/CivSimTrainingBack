using cst_back.Interceptors;
using cst_back.Models;
using cst_back.Services;
using cst_back.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;

namespace cst_back.specs.Fixtures
{
    public static class ServersFixtures
    {
        public static TestServer GetAuthServer(Auth.AuthBase authService)
        {
            var serviceDefinition = Auth.BindService(authService);

            Mock<IAccountDBService> mockAccountDBService = new();
            mockAccountDBService
                .Setup(x => x.GetAccountByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Account>());
            mockAccountDBService
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Account>());

            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddGrpc(options =>
                    {
                        options.Interceptors.Add<ServerInterceptor>();
                    });

                    services.AddSingleton(mockAccountDBService.Object);
                    services.AddScoped<IValidator<CreateAccountRequest>, CreateAccountValidator>();
                    services.AddSingleton(serviceDefinition);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<AuthService>();
                    });
                }));
        }
    }
}
