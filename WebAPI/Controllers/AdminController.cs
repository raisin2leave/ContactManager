using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.EntityFramework.Entities;
using AppCore.Authorization;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Administrator")]
public class AdminController(
    UserManager<CrmUser> userManager,
    RoleManager<CrmRole> roleManager) : ControllerBase
{
    // 🔹 GET ALL USERS
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = userManager.Users.Select(u => new
        {
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.Department,
            u.Status,
            u.IsBlocked
        });

        return Ok(users);
    }

    // 🔹 BLOCK USER
    [HttpPost("users/{id}/block")]
    public async Task<IActionResult> BlockUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.Block();
        await userManager.UpdateAsync(user);

        return Ok("User blocked");
    }

    // 🔹 UNBLOCK USER
    [HttpPost("users/{id}/unblock")]
    public async Task<IActionResult> UnblockUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.Unblock();
        await userManager.UpdateAsync(user);

        return Ok("User unblocked");
    }

    // 🔹 DEACTIVATE USER
    [HttpPost("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.Deactivate(DateTime.UtcNow);
        await userManager.UpdateAsync(user);

        return Ok("User deactivated");
    }

    // 🔹 ACTIVATE USER
    [HttpPost("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.Activate();
        await userManager.UpdateAsync(user);

        return Ok("User activated");
    }

    // 🔹 ASSIGN ROLE
    [HttpPost("users/{id}/roles/{role}")]
    public async Task<IActionResult> AssignRole(string id, string role)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (!await roleManager.RoleExistsAsync(role))
            return BadRequest("Role does not exist.");

        await userManager.AddToRoleAsync(user, role);

        return Ok("Role assigned");
    }

    // 🔹 REMOVE ROLE
    [HttpDelete("users/{id}/roles/{role}")]
    public async Task<IActionResult> RemoveRole(string id, string role)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await userManager.RemoveFromRoleAsync(user, role);

        return Ok("Role removed");
    }
}