using ProxyService.Models;

namespace ProxyService.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
    }
}
