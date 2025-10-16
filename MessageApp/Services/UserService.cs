using MessageApp.DTOs;
using MessageApp.Interfaces;
using MessageApp.Models;
using MessageApp.Repositories;

namespace MessageApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;

        public UserService(IUserRepository userRepository, IAuthenticationService authenticationService)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
        }

        public async Task<UserDto?> Register(CreateUserDto createUserDto)
        {
            // Check if username already exists using repository
            var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
            if (existingUser != null)
                return null;

            var user = new User
            {
                Username = createUserDto.Username,
                Password = createUserDto.Password,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                JoinDate = DateTime.UtcNow
            };

            // Hash password and add salt
            user = _authenticationService.CreateUserCredentials(user);

            await _userRepository.AddAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JoinDate = user.JoinDate,
                LastLogin = user.LastLogin
            };
        }

        public async Task<UserDto?> Login(LoginDto loginDto)
        {
            var user = await _authenticationService.Authenticate(loginDto.Username, loginDto.Password);
            if (user == null)
                return null;

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JoinDate = user.JoinDate,
                LastLogin = user.LastLogin
            };
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JoinDate = user.JoinDate,
                LastLogin = user.LastLogin
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                JoinDate = u.JoinDate,
                LastLogin = u.LastLogin
            });
        }
    }
}