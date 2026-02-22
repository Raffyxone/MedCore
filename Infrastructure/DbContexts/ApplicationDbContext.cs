using Application.Features.User.Domain.Entities;
using Application.Features.User.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    value => Email.Create(value).Value
                )
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(u => u.Email).IsUnique();
        });
    }
}
