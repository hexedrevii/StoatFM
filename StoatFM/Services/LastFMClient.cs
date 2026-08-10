using System.Text.Json;
using StoatFM.Models;

namespace StoatFM.Services;

public class LastFMClient : IDisposable
{
  private readonly HttpClient Client = new HttpClient()
  {
    BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0")
  };

  private readonly string Key;

  public LastFMClient(IConfiguration config)
  {
    string? key = config["LastFMKey"]
      ?? throw new MissingFieldException("No LastFM API key provided.");

    Key = key;
  }

  /// <summary>
  /// Get only the most recent track.
  /// </summary>
  /// <param name="user">LastFM username (not stoat!)</param>
  public async Task<RecentTrack> GetRecentTrackAsync(string user)
  {
    try
    {
      using HttpResponseMessage response = await Client.GetAsync($"?method=user.getrecenttracks&user={user}&api_key={Key}&format=json&limit=1");

      response.EnsureSuccessStatusCode();

      string json = await response.Content.ReadAsStringAsync();

      using JsonDocument document = JsonDocument.Parse(json);

      string total = string.Empty;

      JsonElement recents = document.RootElement.GetProperty("recenttracks");
      if (recents.TryGetProperty("@attr", out JsonElement userAttributes))
      {
        if (userAttributes.TryGetProperty("total", out JsonElement totalScrobbles))
        {
          total = totalScrobbles.GetString() ?? string.Empty;
        }
      }

      JsonElement tracks = document.RootElement.GetProperty("recenttracks").GetProperty("track");
      if (tracks.GetArrayLength() > 0)
      {
        JsonElement latest = tracks[0];

        string name = latest.GetProperty("name").GetString() ?? string.Empty;
        string artist = latest.GetProperty("artist").GetProperty("#text").GetString() ?? string.Empty;
        string album = latest.GetProperty("album").GetProperty("#text").GetString() ?? string.Empty;
        string url = latest.GetProperty("url").GetString() ?? string.Empty;

        bool isPlaying = false;

        if (latest.TryGetProperty("@attr", out JsonElement trackAttributes))
        {
          if (trackAttributes.TryGetProperty("nowplaying", out JsonElement playing))
          {
            isPlaying = playing.GetString() == "true";
          }
        }

        return new RecentTrack(name, artist, album, url, total, isPlaying, true, "");
      }

      return new RecentTrack(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, false, "No listening history.");
    }
    catch (Exception e)
    {
      return new RecentTrack(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, false, e.Message);
    }
  }

  public void Dispose()
  {
    Client.Dispose();
  }
}
