using AppCore.Dto;
using AppCore.Entities;
using AppCore.Exceptions;
using AppCore.Repositories;
using AppCore.Services;
using AutoMapper;

namespace Infrastructure.Services;

public class MemoryPersonService : IPersonService
{
    private readonly IContactUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MemoryPersonService(IContactUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // --- LAB 5: TASK 1 - NOTES ---

    public async Task<PersonDto> GetPerson(Guid personId)
    {
        // Using your correct repo method: FindByIdAsync
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        
        if (person == null) 
            throw new ContactNotFoundException($"Person with id={personId} not found!");
        
        return _mapper.Map<PersonDto>(person);
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        // 1. Get person from repo (FindByIdAsync)
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);

        // 2. Throw custom exception if null
        if (person == null) 
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        // 3. Initialize list if null
        person.Notes ??= new List<Note>();

        // 4. Create Note object
        var note = _mapper.Map<Note>(noteDto);
        note.Id = Guid.NewGuid();
        note.CreatedAt = DateTime.UtcNow;

        // 5. Add to list
        person.Notes.Add(note);

        // 6. Update entity
        await _unitOfWork.Persons.UpdateAsync(person);

        // 7. Save changes
        await _unitOfWork.SaveChangesAsync();

        // 8. Return note
        return note;
    }

    // --- LAB 5: TASK 4 - REMOVE NOTE ---

    public async Task<bool> RemoveNoteFromPerson(Guid personId, Guid noteId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null) throw new ContactNotFoundException($"Person with id={personId} not found!");

        var note = person.Notes?.FirstOrDefault(n => n.Id == noteId);
        if (note == null) return false;

        person.Notes.Remove(note);
        await _unitOfWork.Persons.UpdateAsync(person);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // --- OTHER INTERFACE METHODS (To clear Red lines) ---

    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var pagedResult = await _unitOfWork.Persons.FindPagedAsync(page, size);
        var dtos = _mapper.Map<IEnumerable<PersonDto>>(pagedResult.Items).ToList();
        return new PagedResult<PersonDto>(dtos, pagedResult.TotalCount, page, size);
    }

    public async Task<IEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var people = await _unitOfWork.Persons.GetByEmployerAsync(companyId);
        return _mapper.Map<IEnumerable<PersonDto>>(people);
    }

    public async Task<Person> AddPerson(CreatePersonDto personDto)
    {
        var person = _mapper.Map<Person>(personDto);
        var result = await _unitOfWork.Persons.AddAsync(person);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<Person> UpdatePerson(Guid id, UpdatePersonDto personDto)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(id);
        if (person == null) throw new ContactNotFoundException($"Person {id} not found");
        
        _mapper.Map(personDto, person);
        await _unitOfWork.Persons.UpdateAsync(person);
        await _unitOfWork.SaveChangesAsync();
        return person;
    }

    public async Task<PersonDto?> GetById(Guid id)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(id);
        return _mapper.Map<PersonDto>(person);
    }
}