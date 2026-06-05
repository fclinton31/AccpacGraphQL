using Microsoft.EntityFrameworkCore;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class SettingsDbContext : DbContext
{
    public SettingsDbContext(DbContextOptions<SettingsDbContext> options) : base(options)
    {
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.HasIndex(e => e.UserName).IsUnique();
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyKey).IsRequired();
            entity.Property(e => e.CompanyId).IsRequired();
            entity.Property(e => e.UserName).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.HasIndex(e => e.CompanyKey).IsUnique();
        });

        modelBuilder.Entity<UserCompany>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.CompanyId }).IsUnique();
            entity.HasOne(e => e.User).WithMany(e => e.Companies).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Company).WithMany(e => e.Users).HasForeignKey(e => e.CompanyId);
        });
    }
}

public sealed class UserAccount
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "IntegrationUser";
    public ICollection<UserCompany> Companies { get; set; } = new List<UserCompany>();
}

public sealed class Company
{
    public long Id { get; set; }
    public string CompanyKey { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ICollection<UserCompany> Users { get; set; } = new List<UserCompany>();
}

public sealed class UserCompany
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public UserAccount? User { get; set; }
    public long CompanyId { get; set; }
    public Company? Company { get; set; }
}

