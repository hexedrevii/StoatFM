using StoatSharp.Commands;

namespace StoatFM.Commands;

public class Text : ModuleBase
{
  [Command("ping")]
  public async Task Ping()
  {
    await ReplyAsync("Pong!");
  }
}
