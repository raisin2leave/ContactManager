using AppCore.Entities;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfOrganizationRepository 
    : EfGenericRepository<Organization>, IOrganizationRepository
{
    private readonly ContactsDbContext _context;

    public EfOrganizationRepository(ContactsDbContext context)
        : base(context.Organizations)
    {
        _context = context;
    }

    public Task<IEnumerable<Organization>> GetByTypeAsync(OrganizationType type)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        return await _context.People
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync();
    }
}
