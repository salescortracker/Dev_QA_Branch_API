using BusinessLayer.Common;
using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;

namespace BusinessLayer.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(UserCreateDto userDto);
        Task<User?> UpdateUserAsync(int id, User updatedUser);
        Task<bool> DeleteUserAsync(int id);
        Task<object?> VerifyLoginAsync(string username, string password);
        Task SendWelcomeEmailAsync(User user, string password);
        Task<ApiResponse<bool>> ChangePasswordAsync(PasswordChangeDto dto);
    }
}
