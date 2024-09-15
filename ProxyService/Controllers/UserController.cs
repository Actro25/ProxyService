using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProxyService.Interfaces;
using ProxyService.Models;
using Serilog;
using System.Text.Json;

namespace ProxyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            Log.Information("Request for user with ID {Id}", id);
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                Log.Warning("User with ID {Id} not found", id);
                return NotFound($"User with id {id} not found.");
            }
            Log.Warning("User with ID {Id} not found", id);
            return Ok(user);
        }
    }
}
