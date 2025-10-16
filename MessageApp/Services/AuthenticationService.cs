using MessageApp.Models;
using MessageApp.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MessageApp.Services
{
    public interface IAuthenticationService
    {
        Task<User?> Authenticate(string username, string password);
        User CreateUserCredentials(User user);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> Authenticate(string username, string password)
        {
            User? user;

            user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: user.Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 258 / 8));

            if (hashedPassword != user.Password)
            {
                return null;
            }

            return user;
        }

        public User CreateUserCredentials(User user)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 258 / 8));

            user.Password = hashedPassword;
            user.Salt = salt;
            user.JoinDate = user.JoinDate != DateTime.MinValue ? user.JoinDate : DateTime.Now;
            user.LastLogin = DateTime.Now;

            return user;
        }
    }
}