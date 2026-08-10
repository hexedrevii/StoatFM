using Microsoft.EntityFrameworkCore;
using StoatFM.Models;

namespace StoatFM.Services;

public class SQLite(IConfiguration config) : DbContext
{
  public DbSet<FMUser> Users { get; set; }

  public async Task<FMUser?> GetUserAsync(string id)
  {
    return await Users.Where(u => u.ID == id).FirstOrDefaultAsync();
  }

  public async Task AddUserAsync(string id, string name)
  {
    await Users.AddAsync(
      new FMUser { ID = id, UserName = name }
    );

    await SaveChangesAsync();
  }

  public async Task UpdateUserAsync(string id, string name)
  {
    FMUser? user = await Users.Where(u => u.ID == id).FirstOrDefaultAsync();

    if (user is not null)
    {
      user.UserName = name;
      await SaveChangesAsync();
    }
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    string? db = config["AppSettings:SQLitePath"]
      ?? throw new MissingFieldException("Cannot find SQLite database path");

    optionsBuilder.UseSqlite($"Data Source={db}");
  }
}
