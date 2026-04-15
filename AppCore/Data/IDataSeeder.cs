namespace AppCore.Data;

public interface IDataSeeder
{
    public int Order { get; } 
    Task SeedAsync();
}