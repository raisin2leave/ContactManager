using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AppCore.Validators;
using AppCore.Mapper;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace AppCore.Modules;

public static class ContactsModule
{
    public static IServiceCollection AddContactsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Validators
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();
        
        // 2. UNIVERSAL AUTOMAPPER REGISTRATION
        // This is foolproof because it doesn't use the ambiguous DI extensions
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ContactsMappingProfile>();
        });

        // We create the mapper instance once
        IMapper mapper = mapperConfig.CreateMapper();

        // We register that specific instance as a Singleton
        services.AddSingleton(mapper);
        
        return services;
    }
}