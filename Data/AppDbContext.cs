using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Domain.Categories;
using WorkshowcaseApi.Domain.ProfessionalProfiles;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Domain.Works;

namespace WorkshowcaseApi.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    } 

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProfessionalProfile> ProfessionalProfiles => Set<ProfessionalProfile>();
    public DbSet<Work> Works => Set<Work>();

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

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.NormalizedName)
                .IsUnique();

            entity.HasIndex(x => x.IsActive);
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

            entity.Property(x => x.PrimaryCategoryId)
                .IsRequired();

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

            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.PrimaryCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.ToTable("works");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.CreatedByUserId)
                .IsRequired();

            entity.Property(x => x.PrimaryCategoryId)
                .IsRequired();

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            entity.Property(x => x.CompletionDate);

            entity.Property(x => x.SpaceType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(x => x.CreatedByType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(x => x.HasBeforeAfter)
                .IsRequired();

            entity.Property(x => x.IsAnonymous)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.Property(x => x.PublishedAt);

            entity.HasIndex(x => x.CreatedByUserId);

            entity.HasIndex(x => x.PrimaryCategoryId);

            entity.HasIndex(x => x.Status);

            entity.HasIndex(x => x.PublishedAt);

            entity.HasIndex(x => new { x.Status, x.PublishedAt });

            entity.HasIndex(x => new { x.CreatedByUserId, x.CreatedAt });

            entity.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.PrimaryCategory)
                .WithMany()
                .HasForeignKey(x => x.PrimaryCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
