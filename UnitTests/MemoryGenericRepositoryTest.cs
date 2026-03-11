using AppCore.Entities;
using Infrastructure.Memory;
using Xunit;

namespace UnitTests;

public class MemoryGenericRepositoryTest
{
    private readonly MemoryGenericRepository<Person> _repo = new();

    [Fact]
    public async Task AddAsync_ShouldAddPersonAndReturnIt()
    {
        var person = new Person 
        { 
            FirstName = "Adam", 
            LastName = "Kowalski",
            Email = "adam@example.com" 
        };

        var result = await _repo.AddAsync(person);
        var found = await _repo.FindByIdAsync(person.Id);

        Xunit.Assert.NotNull(found);
        Xunit.Assert.Equal(person.Id, found.Id);
        Xunit.Assert.Equal("Adam", found.FirstName);
    }

    [Fact]
    public async Task FindAllAsync_ShouldReturnAllItems()
    {
        await _repo.AddAsync(new Person { FirstName = "User1" });
        await _repo.AddAsync(new Person { FirstName = "User2" });

        var all = await _repo.FindAllAsync();

        Xunit.Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingEntity()
    {
        var person = new Person { FirstName = "Original" };
        await _repo.AddAsync(person);
        
        person.FirstName = "Updated";
        await _repo.UpdateAsync(person);
        var result = await _repo.FindByIdAsync(person.Id);

        Xunit.Assert.Equal("Updated", result?.FirstName);
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldDeleteEntity()
    {
        var person = new Person { FirstName = "To Delete" };
        await _repo.AddAsync(person);

        await _repo.RemoveByIdAsync(person.Id);
        var result = await _repo.FindByIdAsync(person.Id);

        Xunit.Assert.Null(result);
    }

    [Fact]
    public async Task FindPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 5; i++)
        {
            await _repo.AddAsync(new Person { FirstName = $"Person {i}" });
        }

        var result = await _repo.FindPagedAsync(1, 2);

        Xunit.Assert.Equal(2, result.Items.Count);
        Xunit.Assert.Equal(5, result.TotalCount);
        Xunit.Assert.Equal(3, result.TotalPages);
        Xunit.Assert.True(result.HasNext);
        Xunit.Assert.False(result.HasPrevious);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenEntityDoesNotExist()
    {
        var nonExistentPerson = new Person { Id = Guid.NewGuid(), FirstName = "Ghost" };

        await Xunit.Assert.ThrowsAsync<KeyNotFoundException>(() => _repo.UpdateAsync(nonExistentPerson));
    }
}