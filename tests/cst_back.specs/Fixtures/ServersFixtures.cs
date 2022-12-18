using cst_back.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace cst_back.specs.Fixtures
{
    public static class ServersFixtures
    {
        public static TestServer GetAuthServer(IAuthService authService)
        {
            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddGrpc();
                    services.AddRouting();
                })
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton(authService);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<IAuthService>();
                    });
                }));
        }

        public static void test()
        {
            //var server = GetAuthServer();
            //var channel = GrpcChannel.ForAddress(server.BaseAddress);
        }
    }
}
