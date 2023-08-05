using Model;

namespace Email
{
    public interface IEmailRepository
    {
        EmailResponse SendEmail(string toEmail, string emailSubject, string emailContent);
    }
}
