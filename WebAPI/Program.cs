using AppCore.Modules;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.Memory;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddContactsModule(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
builder.Services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
builder.Services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();
builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
builder.Services.AddSingleton<IPersonService, MemoryPersonService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();