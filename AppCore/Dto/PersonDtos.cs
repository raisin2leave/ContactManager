using AppCore.Entities;

namespace AppCore.Dto;

public record PersonDto : ContactDtos
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    
    public string FullName { get; init; } = string.Empty;
    public string? EmployerName { get; init; }
    
    public string? Position { get; init; }
    public DateTime? BirthDate { get; init; }
    public Gender Gender { get; init; }
    public Guid? EmployerId { get; init; }
    public List<NoteDto>? Notes { get; set; } = new();
}

public record CreatePersonDto(
    string FirstName, 
    string LastName, 
    string Email, 
    string Phone, 
    string? Position, 
    DateTime? BirthDate, 
    Gender Gender, 
    Guid? EmployerId, 
    AddressDto? Address);

public record UpdatePersonDto(
    string? FirstName, 
    string? LastName, 
    string? Email, 
    string? Phone, 
    string? Position, 
    DateTime? BirthDate, 
    Gender? Gender, 
    Guid? EmployerId, 
    AddressDto? Address, 
    ContactStatus? Status);