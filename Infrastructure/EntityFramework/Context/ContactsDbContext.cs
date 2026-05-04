using AppCore.Entities;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

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
        var person1Id = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f");
        var person2Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var person3Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var person4Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var person5Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

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
        builder.Entity<Person>().HasData(
            new
            {
                Id = person1Id,
                FirstName = "Adam",
                LastName = "Nowak",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "adam@wsei.edu.pl",
                Phone = "123456789",
                BirthDate = new DateTime(2001, 1, 11),
                Position = "Programista",
                CreatedAt = new DateTime(2024, 1, 1),
                EmployerId = companyId
            },
            new
            {
                Id = person2Id,
                FirstName = "Anna",
                LastName = "Kowalska",
                Gender = Gender.Female,
                Status = ContactStatus.Active,
                Email = "anna.kowalska@wsei.edu.pl",
                Phone = "500600700",
                BirthDate = new DateTime(1998, 5, 10),
                Position = "HR Specialist",
                CreatedAt = new DateTime(2024, 1, 2),
                EmployerId = companyId
            },
            new
            {
                Id = person3Id,
                FirstName = "Piotr",
                LastName = "Zieliński",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "piotr.zielinski@wsei.edu.pl",
                Phone = "700800900",
                BirthDate = new DateTime(1990, 7, 20),
                Position = "Manager",
                CreatedAt = new DateTime(2024, 1, 3),
                EmployerId = companyId
            },
            new
            {
                Id = person4Id,
                FirstName = "Kasia",
                LastName = "Lewandowska",
                Gender = Gender.Female,
                Status = ContactStatus.Active,
                Email = "kasia.lewandowska@wsei.edu.pl",
                Phone = "111222333",
                BirthDate = new DateTime(1995, 3, 15),
                Position = "Designer",
                CreatedAt = new DateTime(2024, 1, 4),
                EmployerId = companyId
            },
            new
            {
                Id = person5Id,
                FirstName = "Marek",
                LastName = "Wiśniewski",
                Gender = Gender.Male,
                Status = ContactStatus.Inactive,
                Email = "marek.wisniewski@wsei.edu.pl",
                Phone = "999888777",
                BirthDate = new DateTime(1988, 11, 30),
                Position = "Support",
                CreatedAt = new DateTime(2024, 1, 5),
                EmployerId = companyId
            }
        );

// ---- Address (owned type) ----
        builder.Entity<Contact>()
            .OwnsOne(c => c.Address)
            .HasData(
                new
                {
                    ContactId = person1Id,
                    Street = "ul. Kwiatowa 1",
                    City = "Kraków",
                    PostalCode = "30-001",
                    Country = "Poland",
                    Type = AddressType.Main
                },
                new
                {
                    ContactId = person2Id,
                    Street = "ul. Polna 5",
                    City = "Warszawa",
                    PostalCode = "00-001",
                    Country = "Poland",
                    Type = AddressType.Main
                },
                new
                {
                    ContactId = person3Id,
                    Street = "ul. Długa 10",
                    City = "Gdańsk",
                    PostalCode = "80-001",
                    Country = "Poland",
                    Type = AddressType.Main
                },
                new
                {
                    ContactId = person4Id,
                    Street = "ul. Leśna 3",
                    City = "Wrocław",
                    PostalCode = "50-001",
                    Country = "Poland",
                    Type = AddressType.Main
                },
                new
                {
                    ContactId = person5Id,
                    Street = "ul. Słoneczna 9",
                    City = "Łódź",
                    PostalCode = "90-001",
                    Country = "Poland",
                    Type = AddressType.Main
                }
            );
    }
}