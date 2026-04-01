using AppCore.Entities;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfCompanyRepository 
    : EfGenericRepository<Company>, ICompanyRepository
{
    private readonly ContactsDbContext _context;

    public EfCompanyRepository(ContactsDbContext context)
        : base(context.Companies)
    {
        _context = context;
    }

    public Task<IEnumerable<Company>> FindByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Company?> GetByNipAsync(string nip)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Name == nip);
    }

    public async Task<IEnumerable<Company>> SearchByNameAsync(string name)
    {
        return await _context.Companies
            .Where(c => c.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId)
    {
        return await _context.People
            .Where(p => p.EmployerId == companyId)
            .ToListAsync();
    }
}
