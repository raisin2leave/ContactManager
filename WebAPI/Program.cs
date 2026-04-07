using AppCore.Mapper;
using Infrastructure;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers
builder.Services.AddControllers();

// 2. AutoMapper
builder.Services.AddAutoMapper(typeof(ContactsMappingProfile));

// 3. Infrastructure (DbContext, Identity, etc.)
builder.Services.AddContactsEfModule(builder.Configuration);

// 4. Exception handling
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// 🔥 THIS FIXES YOUR ERROR
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();

    // Optional (DEV ONLY - reset DB every run)
    // db.Database.EnsureDeleted();

    db.Database.Migrate();
}

// 5. Middleware
app.UseExceptionHandler();
app.MapControllers();

app.Run();