using Sauvio.Business.Dto;
using Sauvio.Business.Exceptions;
using Sauvio.Business.Services.Email;
using SuavioData.Interfaces;
using SauvioData.Entities.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Sauvio.Business.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAccountData _data;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;

        public AccountService(IAccountData data, IEmailService email, IConfiguration config)
        {
            _data = data;
            _email = email;
            _config = config;
        }


        public async Task Register(RegisterDTO dto)
        {
            if (await _data.GetByEmail(dto.Email) != null)
                throw new ValidationException("Email already registered");

            var token = Guid.NewGuid().ToString();

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ConfirmationToken = token
            };

            await _data.CreateUser(user);
            await _email.SendConfirmationEmail(dto.Email, token);
        }
        public async Task ConfirmEmail(string token)
        {
            var user = await _data.GetByToken(token)
                ?? throw new ValidationException("Invalid or expired token");

            await _data.ConfirmUser(user.Id);
        }


        public async Task<(string Token, User User)> Login(LoginDTO dto)
        {
            var user = await _data.GetByEmail(dto.Email)
                ?? throw new AuthenticationException("Invalid credentials");

            if (!user.IsConfirmed)
                throw new AuthenticationException("Email not confirmed");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new AuthenticationException("Invalid credentials");

            var token = GenerateJwtToken(user);
            return (token, user);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task ChangePassword(ChangePasswordDTO dto)
        {
            var user = await _data.GetById(dto.UserId)
                ?? throw new NotFoundException("User", dto.UserId);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _data.UpdatePassword(user.Id, hashedPassword);
        }


        public async Task<List<User>> GetAllUsers()
        {
            return await _data.GetAllUsers();
        }

        public async Task<User> GetUserById(int userId)
        {
            var user = await _data.GetById(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            return user;
        }

        public async Task PromoteToAdmin(int userId)
        {
            var user = await _data.GetById(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            if (user.IsAdmin)
                throw new InvalidOperationException("User is already an admin");

            await _data.SetAdmin(userId, true);
        }
    }
}
