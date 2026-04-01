using Microsoft.AspNetCore.Identity;
using AppCore;

namespace Infrastructure.EntityFramework.Entities;

public class CrmRole : IdentityRole
{
    public string? Description { get; set; }
    public CrmRole() { }
    public CrmRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
    }
}