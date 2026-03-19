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
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();
   
        var mapperConfig = new MapperConfiguration(cfg => {
            cfg.AddProfile<ContactsMappingProfile>();
        });
        
        services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
        
        return services;
    }
}