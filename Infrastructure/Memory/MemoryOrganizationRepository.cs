using AppCore.Entities;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryOrganizationRepository : MemoryGenericRepository<Organization>, IOrganizationRepository
{
    public Task<IEnumerable<Organization>> GetByTypeAsync(OrganizationType type) => throw new NotImplementedException();
    public Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId) => throw new NotImplementedException();
}