using AppCore.Entities;
using AppCore.Dto;

namespace AppCore.Repositories;

public interface IContactRepository : IGenericRepositoryAsync<Contact>
{
    Task<PagedResult<Contact>> SearchAsync(ContactSearchDto search);
    Task<IEnumerable<Contact>> GetByTagAsync(string tag);
    Task AddNoteAsync(Guid contactId, Note note);
    Task<IEnumerable<Note>> GetNotesAsync(Guid contactId);
    Task AddTagAsync(Guid contactId, Tag tag);
    Task RemoveTagAsync(Guid contactId, Guid tagId);
}