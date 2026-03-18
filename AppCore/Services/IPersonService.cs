using AppCore.Dto;
using AppCore.Entities;

namespace AppCore.Services;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<IEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId);
    Task<Person> AddPerson(CreatePersonDto personDto);
    Task<Person> UpdatePerson(Guid id, UpdatePersonDto personDto);
    Task<PersonDto?> GetById(Guid id);
}