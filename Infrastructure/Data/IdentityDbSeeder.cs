using AppCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Infrastructure.EntityFramework.Entities;
using AppCore.Entities;
using AppCore.Data;

namespace Infrastructure.Data;

public class IdentityDbSeeder(
    UserManager<CrmUser> userManager,
    RoleManager<CrmRole> roleManager,
    ILogger<IdentityDbSeeder> logger) : IDataSeeder
{
    public int Order => 1; // Run first

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new CrmRole(UserRole.Administrator.ToString(), "Full system access."),
            new CrmRole(UserRole.SalesManager.ToString(), "Management access."),
            new CrmRole(UserRole.Salesperson.ToString(), "Sales access."),
            new CrmRole(UserRole.SupportAgent.ToString(), "Support access."),
            new CrmRole(UserRole.ReadOnly.ToString(), "Read only access.")
        };

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role.Name!)) continue;
            await roleManager.CreateAsync(role);
        }
    }

    private async Task SeedUsersAsync()
    {
        // We only create the Admin here as an example; you can add others like the task suggests
        var adminEmail = "admin@crm.pl";
        if (await userManager.FindByEmailAsync(adminEmail) != null) return;

        var admin = new CrmUser
        {
            Id = "F5BADE14-6CC8-42A2-9A44-9842DA2D9280",
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Adam",
            LastName = "Admin",
            FullName = "Adam Admin",
            Department = "IT",
            Status = SystemUserStatus.Active,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, UserRole.Administrator.ToString());
            logger.LogInformation("Seeded Admin user.");
        }
    }
}

// Helper record used internally for organization
internal record SeedUser(string Id, string Email, string FirstName, string LastName, string Department, string Password, UserRole Role);