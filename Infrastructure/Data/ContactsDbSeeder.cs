using AppCore.Data;
using AppCore.Entities;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ContactsDbSeeder(ContactsDbContext context) : IDataSeeder
{
    public int Order => 2; // Run after Identity

    public async Task SeedAsync()
    {
        if (await context.People.AnyAsync()) return; // Already has data

        var company = new Company
        {
            Id = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271"),
            Name = "WSEI University",
            Industry = "Education",
            Status = ContactStatus.Active
        };

        var person = new Person
        {
            Id = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f"),
            FirstName = "Jan",
            LastName = "Kowalski",
            Email = "jan.kowalski@crm.pl",
            Status = ContactStatus.Active,
            Employer = company
        };

        await context.Companies.AddAsync(company);
        await context.People.AddAsync(person);
        await context.SaveChangesAsync();
    }
}