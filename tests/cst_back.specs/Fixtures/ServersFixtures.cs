using cst_back.DBServices;
using cst_back.Interceptors;
using cst_back.Models;
using cst_back.Services;
using cst_back.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Diagnostics.Metrics;

namespace cst_back.specs.Fixtures
{
    public static class ServersFixtures
    {
        public static TestServer GetAuthServer(Auth.AuthBase authService)
        {
            var serviceDefinition = Auth.BindService(authService);

            Mock<ICounterDBService> mockCounterDBService = new();
            Mock<IAccountDBService> mockAccountDBService = new();
            mockAccountDBService
                .Setup(x => x.GetAccountByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Account>());
            mockAccountDBService
                .Setup(x => x.GetAccountByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Account>());
            mockAccountDBService
                .Setup(x => x.InsertAccountAsync(It.IsAny<Account>()))
                .ReturnsAsync(1);

            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddGrpc(options =>
                    {
                        options.Interceptors.Add<ServerInterceptor>();
                    });

                    services.AddSingleton(mockCounterDBService.Object);
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
