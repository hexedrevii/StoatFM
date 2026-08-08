using StoatSharp;

namespace StoatFM.Services;

public class StoatFMClient(ILogger<StoatFMClient> logger, IConfiguration config, StoatClient client, CommandHandler commands) : IHostedService
{
  #region Events

  private void OnReady(SelfUser user)
  {
    logger.LogInformation("Client {}#{} is connected.", user.CurrentName, user.Discriminator);
  }

  #endregion

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    string? token = config["Token"]
      ?? throw new InvalidOperationException("No Token provided.");

    await client.LoginAsync(token, AccountType.Bot);

    client.OnReady += OnReady;

    await commands.LoadAsync();

    await client.StartAsync();
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Client is being disconnected, goodbye!");
    await client.StopAsync();
  }
}
