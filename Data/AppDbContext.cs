using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    } 

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.Phone)
                .HasMaxLength(30);

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.ProfileImageUrl);

            entity.Property(x => x.UserType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => new { x.UserType, x.Status });
        });
    }
}
