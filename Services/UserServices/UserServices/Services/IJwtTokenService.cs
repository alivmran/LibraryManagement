using Library.UserServices.Models;

namespace Library.UserServices.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
