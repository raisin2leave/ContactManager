using AppCore.Data;       
using AppCore.Mapper;
using Infrastructure;
using Infrastructure.EntityFramework.Context;
using Infrastructure.Security; 
using Microsoft.EntityFrameworkCore;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers
builder.Services.AddControllers();

// 2. AutoMapper
builder.Services.AddAutoMapper(typeof(ContactsMappingProfile));

// 3. Infrastructure (DbContext, Identity, Repositories, etc.)
builder.Services.AddContactsEfModule(builder.Configuration);

// 4. JWT Configuration & Registration
// We read the settings from appsettings.json and register them
var jwtSettings = new JwtSettings(builder.Configuration);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddJwt(jwtSettings); // This calls the method we added to ContactsInfrastructureModule

// 5. Exception handling
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// --- DATABASE AUTO-MIGRATION & SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // A. Migrate the Database
    var db = services.GetRequiredService<ContactsDbContext>();
    // Optional (DEV ONLY - reset DB every run)
    // db.Database.EnsureDeleted();
    db.Database.Migrate();

    // B. Run Seeders (Development Only)
    if (app.Environment.IsDevelopment())
    {
        // This finds all classes that implement IDataSeeder (IdentityDbSeeder, ContactsDbSeeder, etc.)
        var seeders = services.GetServices<IDataSeeder>()
                              .OrderBy(s => s.Order);

        foreach (var seeder in seeders)
        {
            // We use 'await' because SeedAsync is an asynchronous task
            await seeder.SeedAsync();
        }
    }
}

// 6. Middleware Pipeline (ORDER IS CRITICAL)
app.UseExceptionHandler();

// Ensure the request flows through security before reaching the controllers
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();