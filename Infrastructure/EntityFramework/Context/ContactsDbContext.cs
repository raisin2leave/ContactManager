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

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=crm.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ========================
        // Identity config
        // ========================
        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<CrmRole>(entity => { entity.Property(r => r.Name).HasMaxLength(20); });

        // ========================
        // TPH (Inheritance)
        // ========================
        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        // ========================
        // Base Contact config
        // ========================
        builder.Entity<Contact>(entity =>
        {
            entity.Property(c => c.Email).HasMaxLength(200);
            entity.Property(c => c.Phone).HasMaxLength(20);

            entity.Property(c => c.Status).HasConversion<string>();

            // IMPORTANT: configure owned type ONCE here
            entity.OwnsOne(c => c.Address);
        });

        // ========================
        // Person config
        // ========================
        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>();

            entity.HasOne(p => p.Employer)
                .WithMany(e => e.Employees)
                .HasForeignKey(p => p.EmployerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Organization)
                .WithMany(o => o.Members)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ========================
        // Organization config
        // ========================
        builder.Entity<Organization>(entity =>
        {
            entity.HasOne(o => o.PrimaryContact)
                .WithMany()
                .HasForeignKey("PrimaryContactId");
        });

        // ========================
        // 🔥 SEEDING (CORRECT WAY)
        // ========================

        var companyId = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271");
        var personId = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f");

// ---- Company ----
        builder.Entity<Company>().HasData(new
        {
            Id = companyId,
            Name = "WSEI",
            Industry = "Edukacja",
            Phone = "123567123",
            Email = "biuro@wsei.edu.pl",
            Website = "https://wsei.edu.pl",
            Status = ContactStatus.Active,
            CreatedAt = new DateTime(2024, 1, 1) // ✅ FIXED
        });

// ---- Person ----
        builder.Entity<Person>().HasData(new
        {
            Id = personId,
            FirstName = "Adam",
            LastName = "Nowak",
            Gender = Gender.Male,
            Status = ContactStatus.Active,
            Email = "adam@wsei.edu.pl",
            Phone = "123456789",
            BirthDate = new DateTime(2001, 1, 11),
            Position = "Programista",
            CreatedAt = new DateTime(2024, 1, 1), // ✅ FIXED
            EmployerId = companyId
        });

// ---- Address (owned type) ----
        builder.Entity<Contact>()
            .OwnsOne(c => c.Address)
            .HasData(
                new
                {
                    ContactId = companyId,
                    Street = "ul. Św. Filipa 17",
                    City = "Kraków",
                    PostalCode = "31-150",
                    Country = "Poland",
                    Type = AddressType.Main
                },
                new
                {
                    ContactId = personId,
                    Street = "ul. Św. Filipa 17",
                    City = "Kraków",
                    PostalCode = "25-009",
                    Country = "Poland",
                    Type = AddressType.Correspondence
                }
            );
    }
}