using StoatSharp.Commands;

namespace StoatFM.Commands;

public class Testing : ModuleBase
{
  [Command("ping")]
  public async Task Ping()
  {
    await ReplyAsync("Pong!");
  }
}
