using ChatServiceV2.Context;
using ChatServiceV2.Dtos;
using ChatServiceV2.Models;
using GenericFileService.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatServiceV2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class AuthController(ApplicationDbContext context) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterDto request, CancellationToken cancellationToken)
        {
            bool isNameExists = await context.Users.AnyAsync(p => p.Name == request.Name, cancellationToken);

            if (isNameExists)
            {
                return BadRequest(new { Message = "This username has been used before." });
            }

            string avatar = FileService.FileSaveToServer(request.File, "wwwroot/avatar/");

            User user = new()
            {
                Name = request.Name,
                Avatar = avatar
            };

            user.Status = "offline";
            await context.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string name, CancellationToken cancellationToken)
        {
            User? user = await context.Users.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);

            if (user is null)
            {
                return BadRequest(new { Message = "User not found." });
            }

            user.Status = "online";

            await context.SaveChangesAsync(cancellationToken);

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(Guid userId, CancellationToken cancellationToken)
        {
            User? user = await context.Users.FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);

            if (user is null)
            {
                return BadRequest(new { Message = "User not found." });
            }

            user.Status = "offline";

            await context.SaveChangesAsync(cancellationToken);

            return Ok(user);
        }

    }
}
