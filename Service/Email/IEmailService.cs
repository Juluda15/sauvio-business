namespace Sauvio.Business.Services.Email
{
    public interface IEmailService
    {
        void SendConfirmationEmail(string toEmail, string token);
    }
}
