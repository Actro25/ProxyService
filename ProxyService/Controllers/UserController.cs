using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProxyService.Models;
using System.Text.Json;

namespace ProxyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        // Кеш для збереження даних про користувачів у пам'яті
        private static readonly Dictionary<int, User> _userCache = new Dictionary<int, User>();

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            // Перевіряємо, чи є користувач в кеші
            if (_userCache.ContainsKey(id))
            {
                return Ok(_userCache[id]);
            }

            // Якщо немає в кеші, робимо запит до зовнішнього API
            var response = await _httpClient.GetAsync($"https://reqres.in/api/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound($"User with id {id} not found.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<ReqresUserResponse>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Додаємо користувача в кеш
            _userCache[id] = userResponse.Data;

            return Ok(userResponse.Data);
        }
    }
}
