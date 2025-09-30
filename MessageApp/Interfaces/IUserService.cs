using MessageApp.DTOs;

namespace MessageApp.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> Register(CreateUserDto createUserDto);
        Task<UserDto?> Login(LoginDto loginDto);
        Task<UserDto?> GetUserById(int id);
        Task<IEnumerable<UserDto>> GetAllUsers();
    }
}