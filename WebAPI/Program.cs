using AppCore.Modules;
using AppCore.Services;
using Infrastructure.Services;
using AppCore.Repositories;
using Infrastructure.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddContactsModule(builder.Configuration);

builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
builder.Services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
builder.Services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();
builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
builder.Services.AddSingleton<IPersonService, MemoryPersonService>();

builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();
app.Run();

app.MapControllers();

app.Run();