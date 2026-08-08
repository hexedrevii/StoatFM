using System.Reflection;
using StoatFM;
using StoatFM.Services;
using StoatSharp;
using StoatSharp.Commands;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// User secrets
// Loaded using secrets.json that is handled by .NET itself!
// dotnet user-secrets set "Token" "YOUR_TOKEN_HERE"
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton(new StoatClient(ClientMode.WebSocket));

builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<CommandHandler>();

// TODO: Database

// Bot service
builder.Services.AddHostedService<StoatFMClient>();

IHost host = builder.Build();
host.Run();
