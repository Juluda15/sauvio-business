namespace Sauvio.Business.Services.Email
{
    public interface IEmailService
    {
        Task SendConfirmationEmail(string toEmail, string token);
    }
}
