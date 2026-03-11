using AppCore.Entities;

namespace AppCore.Dto;

public record PersonDto : ContactDtos
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Position { get; init; }
    public DateTime? BirthDate { get; init; }
    public Gender Gender { get; init; }
    public Guid? EmployerId { get; init; }

    public static PersonDto FromEntity(Person person) => new()
    {
        Id = person.Id,
        FirstName = person.FirstName,
        LastName = person.LastName,
        Email = person.Email,
        Phone = person.Phone,
        Position = person.Position,
        BirthDate = person.BirthDate,
        Gender = person.Gender,
        Status = person.Status,
        CreatedAt = person.CreatedAt,
        Tags = person.Tags.Select(t => t.Name).ToList(),
        EmployerId = person.Employer?.Id,
        Address = new AddressDto(
            person.Address.Street,
            person.Address.City,
            person.Address.PostalCode,
            person.Address.Country,
            person.Address.Type)
    };
}

public record CreatePersonDto(string FirstName, string LastName, string Email, string Phone, string? Position, DateTime? BirthDate, Gender Gender, Guid? EmployerId, AddressDto? Address);

public record UpdatePersonDto(string? FirstName, string? LastName, string? Email, string? Phone, string? Position, DateTime? BirthDate, Gender? Gender, Guid? EmployerId, AddressDto? Address, ContactStatus? Status);