using cst_back;
using cst_back.Interceptors;
using cst_back.Services;
using cst_back.Settings;
using cst_back.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MongoDB.Driver;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Services.Configure<AccountDatabaseSettings>(
    builder.Configuration.GetSection("AccountStoreDatabase"));

builder.Services.AddSingleton<IAccountDBService, AccountDBService>();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ServerInterceptor>();
});
builder.Services.AddGrpcReflection();

builder.Services.AddScoped<IValidator<CreateAccountRequest>, CreateAccountValidator>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

 var app = builder.Build();

app.MapGrpcService<AuthService>();

IWebHostEnvironment env = app.Environment;

if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

logger.Information("launching app");

app.Run();
