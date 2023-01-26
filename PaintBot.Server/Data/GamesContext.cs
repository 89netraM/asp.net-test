using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaintBot.Server.Models;

namespace PaintBot.Server.PastGames;

public class GamesContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Bot> Bots { get; set; }

    public GamesContext(DbContextOptions<GamesContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
}
