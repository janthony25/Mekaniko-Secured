namespace Mekaniko_Secured.Services
{
    public interface IEmailService
    {
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string fileName);
    }
}
