using cst_back.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7054, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var app = builder.Build();
app.UseRouting();

app.MapGrpcService<AuthService>();
app.UseEndpoints(endpoints =>
{

    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("GRPC service running...");
    });
});

app.Run();
