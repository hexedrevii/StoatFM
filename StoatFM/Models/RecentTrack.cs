namespace StoatFM.Models;

public record struct RecentTrack(
  string Name, string Artist, string Album, string Url, string Scrobbles, bool Playing,
  bool Success, string What
);
