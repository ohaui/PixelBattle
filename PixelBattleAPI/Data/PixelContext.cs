using Microsoft.EntityFrameworkCore;
using PixelBattleAPI.Model;

namespace PixelBattleAPI.Data;

public sealed class PixelContext : DbContext
{
    public DbSet<Pixel> Pixels { get; set; }
    public DbSet<User> Users { get; set; }

    public PixelContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}