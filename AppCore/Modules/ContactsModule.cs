using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AppCore.Validators;
using AppCore.Mapper;

namespace AppCore.Modules;

public static class ContactsModule
{
    public static IServiceCollection AddContactsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();
        
        // Use the explicit static class call to avoid "Ambiguous invocation"
        // This bypasses the extension method confusion.
        var assemblies = new[] { typeof(ContactsMappingProfile).Assembly };
        
        // Try this specific syntax:
        Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions
            .AddAutoMapper(services, assemblies);

        return services;
    }
}