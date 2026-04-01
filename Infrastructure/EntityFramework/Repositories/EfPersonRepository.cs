using AppCore.Entities;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.EntityFramework.Repositories;

public class EfPersonRepository(ContactsDbContext context) 
    : EfGenericRepository<Person>(context.People), IPersonRepository
{
    public async Task<IEnumerable<Person>> GetByEmployerAsync(Guid companyId) =>
        await context.People.Where(p => p.EmployerId == companyId).ToListAsync();

    public async Task<IEnumerable<Person>> GetByOrganizationAsync(Guid organizationId) =>
        await context.People.Where(p => p.OrganizationId == organizationId).ToListAsync();
}