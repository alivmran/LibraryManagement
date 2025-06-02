using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.UserServices.Data;
using Library.UserServices.DTOs;
using Library.UserServices.Models;
using Library.UserServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.UserServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication by default
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenService _jwtService;

        public UsersController(
            ApplicationDbContext context,
            IJwtTokenService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/Users/login
        // Allows a user to obtain a JWT by supplying valid credentials
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOs.LoginRequest req)
        {
            // In production, you'd hash & salt the password
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == req.Email &&
                    u.PasswordHash == req.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

        // POST: api/Users
        // Registration endpoint
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            // In production, hash the password before storing:
            // user.PasswordHash = _passwordHasher.Hash(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
            => await _context.Users.ToListAsync();

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            return user;
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (
                !_context.Users.Any(e => e.Id == id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
