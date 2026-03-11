using AppCore.Entities;

namespace AppCore.Repositories;

public interface IPersonRepository : IGenericRepositoryAsync<Person>
{
    Task<IEnumerable<Person>> GetByEmployerAsync(Guid companyId);
    Task<IEnumerable<Person>> GetByOrganizationAsync(Guid organizationId);
}