using AppCore.Entities;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    public ContactsDbContext() { }
    public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Temporary hardcoded path for migrations. 
        // We will move this to appsettings.json in Step 6.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=crm.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<CrmRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(20);
        });

        // TPH - Table Per Hierarchy
        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        builder.Entity<Contact>(entity =>
        {
            entity.Property(p => p.Email).HasMaxLength(200);
            entity.Property(p => p.Phone).HasMaxLength(20);
            entity.OwnsOne(c => c.Address); // Address as Owned Type
        });

        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>();
            entity.Property(p => p.Status).HasConversion<string>();
            
            // Relationships
            entity.HasOne(p => p.Employer).WithMany(e => e.Employees);
            entity.HasOne(p => p.Organization).WithMany(o => o.Members);
        });

        // Seed Data
        var companyId = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271");
        builder.Entity<Company>().HasData(new Company()
        {
            Id = companyId,
            Name = "WSEI",
            Industry = "edukacja",
            Phone = "123567123",
            Email = "biuro@wsei.edu.pl",
            Website = "https://wsei.edu.pl"
        });

        var personId = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f");
        builder.Entity<Person>().HasData(new
        {
            Id = personId,
            FirstName = "Adam",
            LastName = "Nowak",
            Gender = Gender.Male,
            Status = ContactStatus.Active,
            Email = "adam@wsei.edu.pl",
            Phone = "123456789",
            BirthDate = DateTime.Parse("2001-01-11"),
            Position = "Programista",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });

        // Seed Address for Adam
        builder.Entity<Contact>().OwnsOne(c => c.Address).HasData(new
        {
            ContactId = personId, // Foreign Key to Adam
            City = "Kraków",
            Country = "Poland",
            PostalCode = "25-009",
            Street = "ul. Św. Filipa 17",
            Type = AddressType.Correspondence
        });
    }
}