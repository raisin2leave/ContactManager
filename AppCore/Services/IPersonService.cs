using AppCore.Dto;

namespace AppCore.Services;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<PersonDto?> FindById(Guid id);
    Task<PersonDto> CreatePerson(CreatePersonDto dto);
    Task UpdatePerson(Guid id, UpdatePersonDto dto);
    Task DeletePerson(Guid id);
}