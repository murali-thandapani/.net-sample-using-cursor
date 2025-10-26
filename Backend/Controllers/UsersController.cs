using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Data;
using UserManagement.API.DTOs;
using UserManagement.API.Helpers;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var userDto = new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(userDto);
    }

    // POST: api/Users
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        // Check if username or email already exists
        if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = new Models.User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Role = createUserDto.Role,
            PasswordHash = PasswordHelper.HashPassword(createUserDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Check if email is being changed and if it's already in use
        if (user.Email != updateUserDto.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id))
            {
                return BadRequest(new { message = "Email already exists" });
            }
        }

        user.Email = updateUserDto.Email;
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.Role = updateUserDto.Role;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            user.PasswordHash = PasswordHelper.HashPassword(updateUserDto.Password);
        }

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
