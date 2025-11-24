
namespace SnackToSixPack.Classes
{
    public class EmailService
    {
        public EmailService()
        {
        }

        internal void SendPasswordResetLink(string? email)
        {
            var emailService = new EmailService();
            emailService.SendPasswordResetLink(email);
        }
    }
}