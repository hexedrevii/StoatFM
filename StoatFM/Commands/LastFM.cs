using Microsoft.EntityFrameworkCore;
using StoatFM.Models;
using StoatFM.Services;
using StoatSharp;
using StoatSharp.Commands;

namespace StoatFM.Commands;

public class LastFM(LastFMClient fm, IDbContextFactory<SQLite> factory) : ModuleBase
{
  [Command("fm")]
  public async Task Fm()
  {
    if (Context is null || Context.User is null)
      return;

    using SQLite db = await factory.CreateDbContextAsync();

    FMUser? user = await db.GetUserAsync(Context.User.Id);
    if (user is null)
    {
      await ReplyAsync("You have not set your LastFM username! Please use .register <name>.");
      return;
    }

    RecentTrack recent = await fm.GetRecentTrackAsync(user.UserName);

    if (!recent.Success)
    {
      await ReplyAsync(recent.What);
      return;
    }

    // Ignoring image because they're fugly in stoat embeds :(

    string name = $"{Context.User.CurrentName}#{Context.User.Discriminator}";

    EmbedBuilder track = new EmbedBuilder
    {
      Title = $"{recent.Name}     ",
      Url = recent.Url,
      Description = $"**{recent.Artist}**{( recent.Album != string.Empty ? $" • *{recent.Album}*" : "")}\n\n###### {(recent.Playing ? "Now playing" : "Played before")}{(name == string.Empty ? "" : $" for {name}")}\n###### {recent.Scrobbles} total scrobbles    "
    };

    await ReplyAsync("", [track.Build()]);
  }

  [Command("register")]
  public async Task Register(string username)
  {
    if (Context is null || Context.User is null) return;

    using SQLite db = await factory.CreateDbContextAsync();

    FMUser? user = await db.GetUserAsync(Context.User.Id);
    if (user is null)
    {
      await db.AddUserAsync(Context.User.Id, username);

      Embed embed = new EmbedBuilder
      {
        Title = "Thank you!",
        Description = $"You are now registered as {username}!"
      }.Build();

      await ReplyAsync("", [embed]);
    }
    else
    {
      await db.UpdateUserAsync(Context.User.Id, username);

      Embed embed = new EmbedBuilder
      {
        Title = "Username updated!",
        Description = $"Your name has been updated to {username}."
      }.Build();

      await ReplyAsync("", [embed]);
    }
  }
}
