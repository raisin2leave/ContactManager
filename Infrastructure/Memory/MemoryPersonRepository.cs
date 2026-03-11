using AppCore.Entities;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryPersonRepository : MemoryGenericRepository<Person>, IPersonRepository
{
    public MemoryPersonRepository() : base()
    {
        var id1 = Guid.NewGuid();
        _data.Add(id1, new Person
        {
            Id = id1,
            FirstName = "Adam",
            LastName = "Nowak",
            Gender = Gender.Male,
            Email = "adam.nowak@example.com",
            Status = ContactStatus.Active
        });

        var id2 = Guid.NewGuid();
        _data.Add(id2, new Person
        {
            Id = id2,
            FirstName = "Anna",
            LastName = "Kowalska",
            Gender = Gender.Female,
            Email = "a.kowalska@test.pl",
            Status = ContactStatus.Prospect
        });
    }

    public Task<IEnumerable<Person>> GetByEmployerAsync(Guid companyId)
    {
        return Task.FromResult(_data.Values.Where(p => p.Employer?.Id == companyId));
    }

    public Task<IEnumerable<Person>> GetByOrganizationAsync(Guid organizationId)
    {
        return Task.FromResult(_data.Values.Where(p => p.Organization?.Id == organizationId));
    }
}