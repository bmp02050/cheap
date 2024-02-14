using cheap.Entities;
using Microsoft.EntityFrameworkCore;

namespace cheap;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public DbSet<User>? Users { get; set; }
    public DbSet<RegistrationInviteToken> RegistrationInviteTokens { get; set; }
    public DbSet<TokenRepository> TokenRepository { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Record> Records { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Record>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.Records)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Record>()
            .HasOne(uf => uf.Location)
            .WithMany()
            .HasForeignKey(uf => uf.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Record>()
            .HasOne(uf => uf.Item)
            .WithMany()
            .HasForeignKey(uf => uf.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}