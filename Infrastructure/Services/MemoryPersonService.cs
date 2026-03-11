using AppCore.Dto;
using AppCore.Entities;
using AppCore.Repositories;
using AppCore.Services;

namespace Infrastructure.Services;

public class MemoryPersonService(IContactUnitOfWork unitOfWork) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var peoplePaged = await unitOfWork.Persons.FindPagedAsync(page, size);
        var dtoItems = peoplePaged.Items.Select(PersonDto.FromEntity).ToList();
        
        return new PagedResult<PersonDto>(dtoItems, peoplePaged.TotalCount, peoplePaged.Page, peoplePaged.PageSize);
    }

    public async Task<PersonDto?> FindById(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        return person != null ? PersonDto.FromEntity(person) : null;
    }

    public async Task<PersonDto> CreatePerson(CreatePersonDto dto)
    {
        var person = new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Gender = dto.Gender,
            Position = dto.Position,
            BirthDate = dto.BirthDate
        };
        
        var saved = await unitOfWork.Persons.AddAsync(person);
        await unitOfWork.SaveChangesAsync();
        return PersonDto.FromEntity(saved);
    }

    public async Task UpdatePerson(Guid id, UpdatePersonDto dto) => throw new NotImplementedException();
    public async Task DeletePerson(Guid id) => await unitOfWork.Persons.RemoveByIdAsync(id);
}