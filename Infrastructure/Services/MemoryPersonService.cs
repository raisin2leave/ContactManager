using AppCore.Dto;
using AppCore.Entities;
using AppCore.Repositories;
using AppCore.Services;
using AutoMapper;

namespace Infrastructure.Services;

public class MemoryPersonService(IContactUnitOfWork unitOfWork, IMapper mapper) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var result = await unitOfWork.Persons.FindPagedAsync(page, size);
        var items = mapper.Map<List<PersonDto>>(result.Items);
        return new PagedResult<PersonDto>(items, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Person> AddPerson(CreatePersonDto personDto)
    {
        var entity = mapper.Map<Person>(personDto);
        var result = await unitOfWork.Persons.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<Person> UpdatePerson(Guid id, UpdatePersonDto personDto)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id) ?? throw new KeyNotFoundException();
        mapper.Map(personDto, entity);
        var result = await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<PersonDto?> GetById(Guid id)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);
        return entity != null ? mapper.Map<PersonDto>(entity) : null;
    }

    public async Task<IEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var items = await unitOfWork.Persons.GetByEmployerAsync(companyId);
        return mapper.Map<IEnumerable<PersonDto>>(items);
    }
}