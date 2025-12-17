using Sauvio.Business.Dto;
using SauvioData.Models;


namespace Sauvio.Business.Services.Account
{
    public interface IAccountService
    {
        Task<string> Register(RegisterDTO dto);
        Task<(bool Success, string Message, User? User)> Login(LoginDTO dto);
        Task<string> ConfirmEmail(string token);
        Task<(bool Success, string Message)> ChangePassword(ChangePasswordDTO dto);
    }

}
