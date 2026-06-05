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
            entity.ToTable("User");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.UserName).IsRequired();
            entity.Property(e => e.UserName).HasColumnName("UserName");
            entity.Property(e => e.Email).HasColumnName("EmailAddress");
            entity.Property(e => e.FullName).HasColumnName("FullName");
            entity.Property(e => e.PasswordHash).IsRequired().HasColumnName("Password");
            entity.Property(e => e.Role).IsRequired().HasColumnName("UserRole");
            entity.HasIndex(e => e.UserName).IsUnique();
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CompanyId).IsRequired().HasColumnName("CmpID");
            entity.Property(e => e.CompanyKey).IsRequired().HasColumnName("Token");
            entity.Property(e => e.UserName).IsRequired().HasColumnName("UserName");
            entity.Property(e => e.Password).IsRequired().HasColumnName("Password");
            entity.Ignore(e => e.ErrorMessage);
            entity.Ignore(e => e.IsChecked);
            entity.HasIndex(e => e.CompanyKey).IsUnique();
        });

        modelBuilder.Entity<UserCompany>(entity =>
        {
            entity.ToTable("UserCompany");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.FullName).HasColumnName("FullName");
            entity.Property(e => e.CompanyCode).HasColumnName("CompID");
            entity.Property(e => e.ApiKey).HasColumnName("APIKey");

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
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "IntegrationUser";
    public string? FullName { get; set; }
    public ICollection<UserCompany> Companies { get; set; } = new List<UserCompany>();
}

public sealed class Company
{
    public long Id { get; set; }
    public string CompanyKey { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public bool IsChecked { get; set; }
    public ICollection<UserCompany> Users { get; set; } = new List<UserCompany>();
}

public sealed class UserCompany
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public UserAccount? User { get; set; }
    public long CompanyId { get; set; }
    public Company? Company { get; set; }
    public string? FullName { get; set; }
    public string? CompanyCode { get; set; }
    public string? ApiKey { get; set; }
}
