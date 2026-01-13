using Sauvio.Business.Dto;
using SauvioData.Entities.User;


namespace Sauvio.Business.Services.Account
{
    public interface IAccountService
    {
        Task Register(RegisterDTO dto);
        Task<(string Token, User User)> Login(LoginDTO dto);
        Task ConfirmEmail(string token);
        Task ChangePassword(ChangePasswordDTO dto);
    }

}
