namespace AuthServer.Infrastructure.Data;

using AuthServer.Core;
using Microsoft.EntityFrameworkCore;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(builder => 
        {
            builder
                .HasMany<AllowedGrantType>()
                .WithOne()
                .HasForeignKey(agt => agt.ClientId);

            builder
                .HasMany<RedirectUri>()
                .WithOne()
                .HasForeignKey(ru => ru.ClientId);

            builder
                .HasMany<AllowedResponseType>()
                .WithOne()
                .HasForeignKey(ru => ru.ClientId);

            builder.Ignore(c => c.AllowedGrantTypes);
            builder.Ignore(c => c.RedirectUris);
            builder.Ignore(c => c.AllowedResponseTypes);
        });

        modelBuilder.Entity<AllowedGrantType>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<RedirectUri>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<AllowedResponseType>(builder =>
        {
            builder.HasNoKey();
        });
    }
}