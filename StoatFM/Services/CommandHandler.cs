using System.Reflection;
using StoatSharp;
using StoatSharp.Commands;

namespace StoatFM.Services;

public class CommandHandler(ILogger<CommandHandler> logger, StoatClient client, CommandService commands, IServiceProvider services, string prefix = "!")
{
  public string Prefix { get; } = prefix;

  private async void MessageRecieved(Message message)
  {
    if (message is not UserMessage userMessage || message.Type != MessageType.User)
      return;

    int pos = 0;
    if (
      !userMessage.HasStringPrefix(Prefix, ref pos) &&
      !userMessage.HasMentionPrefix(client.CurrentUser ?? throw new Exception(), ref pos)
    ) return;

    CommandContext ctx = new CommandContext(client, userMessage);

    await commands.ExecuteAsync(ctx, pos, services);
  }

  public async Task LoadAsync()
  {
    client.OnMessageRecieved += MessageRecieved;

    var loaded = await commands.AddModulesAsync(Assembly.GetEntryAssembly() ?? throw new Exception(), services);
    logger.LogInformation("Loaded {} command{}.", loaded.Count(), loaded.Count() == 1 ? "" : "s");
  }
}
