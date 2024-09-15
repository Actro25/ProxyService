using ProxyService.Interfaces;
using ProxyService.Models;
using Serilog;
using System.Text.Json;

namespace ProxyService.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        // Кеш для збереження користувачів у пам'яті
        private static readonly Dictionary<int, User> _userCache = new Dictionary<int, User>();

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> GetUserById(int id)
        {
            // Перевіряємо, чи є користувач в кеші
            Log.Information("Try to find user in cache with ID {Id}", id);
            if (_userCache.ContainsKey(id))
            {
                Log.Information($"Find user {_userCache[id]} with ID {id}", id);
                return _userCache[id];
            }
            Log.Information("Dont find user with ID {Id}", id);
            // Якщо користувача немає в кеші, виконуємо HTTP-запит до reqres.in
            // Логування запиту до зовнішнього API
            Log.Information("Request for API with ID {Id}", id);
            var response = await _httpClient.GetAsync($"https://reqres.in/api/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("User dont finded with ID {Id}", id);
                return null; // повертаємо null, якщо користувача не знайдено
            }
            Log.Information("Request Is siccess status code with ID {Id}", id);


            var responseContent = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<ReqresUserResponse>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            //var userResponse = JsonSerializer.Deserialize<ReqresUserResponse>(responseContent);



            // Додаємо користувача в кеш
            // Логування додавання користувача в кеш
            Log.Information("Add user in cache with ID {Id}", id);
            _userCache[id] = userResponse.Data;

            return userResponse.Data;
        }
    }
}
