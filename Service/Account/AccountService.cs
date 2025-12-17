using Sauvio.Business.Dto;
using SuavioData.Interfaces;
using SauvioData.Models;
using Sauvio.Business.Services.Email;


namespace Sauvio.Business.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAccountData _data;
        private readonly IEmailService _email;

        public AccountService(IAccountData data, IEmailService email)
        {
            _data = data;
            _email = email;
        }

        public async Task<string> Register(RegisterDTO dto)
        {
            if (await _data.GetByEmail(dto.Email) != null)
                return "Email already registered";

            var token = Guid.NewGuid().ToString();

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ConfirmationToken = token
            };

            await _data.CreateUser(user);
            _email.SendConfirmationEmail(dto.Email, token);

            return "Registration successful. Please check your email.";
        }

        public async Task<string> ConfirmEmail(string token)
        {
            var user = await _data.GetByToken(token);
            if (user == null) return "Invalid or expired token";

            await _data.ConfirmUser(user.Id);
            return "Email confirmed successfully!";
        }

        public async Task<(bool Success, string Message, User? User)> Login(LoginDTO dto)
        {
            var user = await _data.GetByEmail(dto.Email);
            if (user == null || !user.IsConfirmed)
                return (false, "Invalid credentials", null);

            return BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)
                ? (true, "Login successful", user)
                : (false, "Invalid credentials", null);
        }

        public async Task<(bool Success, string Message)> ChangePassword(ChangePasswordDTO dto)
        {
            var user = await _data.GetById(dto.UserId);
            if (user == null) return (false, "User not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _data.UpdatePassword(user.Id, hashedPassword);

            return (true, "Password changed successfully");
        }
    }
}