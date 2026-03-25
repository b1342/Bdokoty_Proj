using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Domain.ProfessionalProfiles;
using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    } 

    public DbSet<User> Users => Set<User>();
    public DbSet<ProfessionalProfile> ProfessionalProfiles => Set<ProfessionalProfile>();

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

        modelBuilder.Entity<ProfessionalProfile>(entity =>
        {
            entity.ToTable("professional_profiles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId)
                .IsRequired();

            entity.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.PrimaryCategory)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.SecondaryCategoriesJson)
                .IsRequired();

            entity.Property(x => x.ServiceAreasJson)
                .IsRequired();

            entity.Property(x => x.ContactPhone)
                .HasMaxLength(30);

            entity.Property(x => x.ContactEmail)
                .HasMaxLength(256);

            entity.Property(x => x.WhatsappNumber)
                .HasMaxLength(30);

            entity.Property(x => x.ContactPreference)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.LogoUrl)
                .HasMaxLength(2048);

            entity.Property(x => x.WebsiteUrl)
                .HasMaxLength(2048);

            entity.Property(x => x.IsPublic)
                .IsRequired();

            entity.Property(x => x.IsVerified)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasIndex(x => x.UserId)
                .IsUnique();

            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<ProfessionalProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
