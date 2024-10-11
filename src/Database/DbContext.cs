using BlogApp.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Database;

public class BlogAppDbContext : DbContext
{
    public DbSet<User> User { get; set; }

    public DbSet<Profile> Profile { get; set; }

    public DbSet<Blog> Blog { get; set; }

    public DbSet<Otp> Otp { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Env.GetString("DATABASE_URL");

        optionsBuilder.UseNpgsql(connectionString);
    }
}
