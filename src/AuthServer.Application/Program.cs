using AuthServer.Infrastructure;
using AuthServer.Core.Interface;
using AuthServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AuthServer.Application.Endpoints.PostClient;
using System.Security.Cryptography;
using AuthServer.Application.Endpoints.PostToken;
using AuthServer.Application.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.WriteTo.File("log/app.txt");
    });

builder.Services.AddConfigOptions(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseInMemoryDatabase("AuthServerDb"));

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(_ => {
    var rsa = RSA.Create(2048);

    return rsa;
});
builder.Services.AddSingleton<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.UseAntiforgery();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<AuthServer.Application.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/client", PostClientEndpoint.HandleAsync);

app.Run();

public partial class Program { }