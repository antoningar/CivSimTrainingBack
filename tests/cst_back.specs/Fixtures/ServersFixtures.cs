using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Interceptors;
using cst_back.Protos;
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
            Mock<ICounterDBService> mockCounterDBService = new();
            Mock<ICryptoHelper> mockCryptoHelper = new();
            mockCryptoHelper
                .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            Mock<IInstanceDBService> mockInstanceDBService = new();
            Mock<IInstanceService> mockInstancesService = new();

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

        public static TestServer GetInstancesServer(
            Mock<IInstanceDBService>? mockInstanceDBService= null,
            Mock<ILeaderboardDBService>? mockLeaderboardService  = null,
            Mock<IAccountDBService>? mockAccountDBService = null,
            Mock<IFileHelper>? mockFileHelper = null,
            Mock<IFileDBService>? mockFileDBService= null)
        {
            mockInstanceDBService = (mockInstanceDBService == null) ? new Mock<IInstanceDBService>() : mockInstanceDBService;
            mockLeaderboardService = (mockLeaderboardService == null) ? new Mock<ILeaderboardDBService>() : mockLeaderboardService;
            mockAccountDBService = (mockAccountDBService == null) ? new Mock<IAccountDBService>() : mockAccountDBService;
            mockFileHelper = (mockFileHelper == null) ? new Mock<IFileHelper>() : mockFileHelper;
            mockFileDBService = (mockFileDBService == null) ? new Mock<IFileDBService>() : mockFileDBService;

            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddGrpc(options =>
                    {
                        options.Interceptors.Add<ServerInterceptor>();
                    });

                    services.AddSingleton(mockInstanceDBService.Object);
                    services.AddSingleton(mockLeaderboardService.Object);
                    services.AddSingleton(mockAccountDBService.Object);
                    services.AddSingleton(mockFileHelper.Object);
                    services.AddSingleton(mockFileDBService.Object);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<InstanceService>();
                    });
                })
            );
        }

        public static TestServer GetFileInfoServer(
            Mock<IFileHelper>? mockFileHelper = null,
            Mock<IAccountDBService>? mockAccountDBService = null,
            Mock<IInstanceDBService>? mockInstanceDBService = null,
            Mock<IFileDBService>? mockFileDBService = null)
        {
            mockFileHelper = (mockFileHelper == null) ? new Mock<IFileHelper>() : mockFileHelper;
            mockAccountDBService = (mockAccountDBService == null) ? new Mock<IAccountDBService>() : mockAccountDBService;
            mockInstanceDBService = (mockInstanceDBService == null) ? new Mock<IInstanceDBService>() : mockInstanceDBService;
            mockFileDBService = (mockFileDBService == null) ? new Mock<IFileDBService>() : mockFileDBService;

            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddGrpc(options =>
                    {
                        options.Interceptors.Add<ServerInterceptor>();
                    });

                    services.AddSingleton(mockFileHelper.Object);
                    services.AddSingleton(mockAccountDBService.Object);
                    services.AddSingleton(mockFileDBService.Object);
                    services.AddSingleton(mockInstanceDBService.Object);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<FileInfoService>();
                    });
                })
            );
        }
    }
}
