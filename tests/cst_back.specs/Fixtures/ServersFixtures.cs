using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Interceptors;
using cst_back.Services;
using cst_back.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace cst_back.specs.Fixtures
{
    public static class ServersFixtures
    {
        public static TestServer GetAuthServer(Auth.AuthBase authService, Mock<IAccountDBService> mockAccountDBService)
        {
            var serviceDefinition = Auth.BindService(authService);

            Mock<ICounterDBService> mockCounterDBService = new();
            Mock<ICryptoHelper> mockCryptoHelper = new();
            mockCryptoHelper
                .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddGrpc(options =>
                    {
                        options.Interceptors.Add<ServerInterceptor>();
                    });

                    services.AddSingleton(mockCounterDBService.Object);
                    services.AddSingleton(mockAccountDBService.Object);
                    services.AddSingleton(mockCryptoHelper.Object);
                    services.AddScoped<IValidator<CreateAccountRequest>, CreateAccountValidator>();
                    services.AddScoped<IValidator<ConnectRequest>, ConnectValidator>();
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
