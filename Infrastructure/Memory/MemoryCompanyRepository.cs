using AppCore.Entities;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryCompanyRepository : MemoryGenericRepository<Company>, ICompanyRepository
{
    public Task<IEnumerable<Company>> FindByNameAsync(string name) => throw new NotImplementedException();
    public Task<Company?> GetByNipAsync(string nip) => throw new NotImplementedException();
    public Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId) => throw new NotImplementedException();
}