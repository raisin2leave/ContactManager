namespace AppCore.Dto;

public record CreateNoteDto(string Content);

public record NoteDto(Guid Id, string Content, DateTime CreatedAt);