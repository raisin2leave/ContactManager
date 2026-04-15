using AppCore;
using AppCore.Repositories;
using AppCore.Services;
using AppCore.Authorization;
using AppCore.Data; // Added for CrmPolicies
using AppCore.Entities;
using Infrastructure.Data; // Added for SystemUserStatus and UserRole
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Added for JWT
using Microsoft.IdentityModel.Tokens;               // Added for TokenValidation
using Microsoft.AspNetCore.Authorization;           // Added for Policies

namespace Infrastructure;

public static class ContactsInfrastructureModule
{
    // --- METHOD 1: EF and Core Services ---
    public static IServiceCollection AddContactsEfModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CrmDb")));

        services.AddIdentity<CrmUser, CrmRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ContactsDbContext>()
            .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<IPersonRepository, EfPersonRepository>();
        services.AddScoped<ICompanyRepository, EfCompanyRepository>(); 
        services.AddScoped<IOrganizationRepository, EfOrganizationRepository>(); 
        
        // Services
        services.AddScoped<IContactUnitOfWork, EfContactsUnitOfWork>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAuthService, AuthService>();
        
        // Register Seeders
        services.AddScoped<IDataSeeder, IdentityDbSeeder>();
        services.AddScoped<IDataSeeder, ContactsDbSeeder>();
        
        return services;
    }

    // --- METHOD 2: JWT and Authorization ---
    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = jwtOptions.GetSymmetricKey(),
                ClockSkew = TimeSpan.Zero 
            };
        });

        services.AddAuthorization(options =>
        {
            // 1. Admin Only
            options.AddPolicy(CrmPolicies.AdminOnly.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));

            // 2. Sales Access (Admin, Manager, or Salesperson)
            options.AddPolicy(CrmPolicies.SalesAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(), 
                    UserRole.SalesManager.ToString(), 
                    UserRole.Salesperson.ToString()));

            // 3. Sales Manager Access (Admin or Manager)
            options.AddPolicy(CrmPolicies.SalesManagerAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(), 
                    UserRole.SalesManager.ToString()));

            // 4. Support Access
            options.AddPolicy(CrmPolicies.SupportAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(), 
                    UserRole.SupportAgent.ToString()));

            // 5. Read Only (Any valid role)
            options.AddPolicy(CrmPolicies.ReadOnlyAccess.ToString(), policy =>
                policy.RequireAuthenticatedUser());

            // 6. Active User Only
            options.AddPolicy(CrmPolicies.ActiveUser.ToString(), policy =>
                policy.RequireAuthenticatedUser().RequireClaim("status", SystemUserStatus.Active.ToString()));

            // 7. Sales Department Only
            options.AddPolicy(CrmPolicies.SalesDepartment.ToString(), policy =>
                policy.RequireClaim("department", "Sales"));
                
            // Default Policy - Everyone must be logged in by default
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Fallback Policy - Applied if [Authorize] attribute is missing
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}