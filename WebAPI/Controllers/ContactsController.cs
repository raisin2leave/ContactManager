using AppCore.Dto;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;
using AppCore.Entities;

namespace WebApi.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPersons(int page = 1, int size = 10) =>
        Ok(await service.FindAllPeoplePaged(page, size));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var dto = await service.GetById(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePersonDto dto)
    {
        var result = await service.AddPerson(dto);
        return CreatedAtAction(nameof(GetPerson), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePersonDto dto)
    {
        try
        {
            var result = await service.UpdatePerson(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    [HttpPost("{contactId:guid}/notes")]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddNote([FromRoute] Guid contactId, [FromBody] CreateNoteDto dto)
    {
        var note = await service.AddNoteToPerson(contactId, dto);
        // Note: Use 'GetNotes' as the action name for the location header
        return CreatedAtAction(nameof(GetNotes), new { contactId }, note);
    }

    [HttpGet("{contactId:guid}/notes")]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        var person = await service.GetPerson(contactId);
        return Ok(person.Notes);
    }

    [HttpDelete("{contactId:guid}/notes/{noteId:guid}")]
    public async Task<IActionResult> DeleteNote(Guid contactId, Guid noteId)
    {
        await service.RemoveNoteFromPerson(contactId, noteId);
        return NoContent();
    }
}