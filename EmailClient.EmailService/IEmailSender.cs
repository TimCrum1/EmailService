namespace EmailClient.EmailService;

/// <summary>
/// Interface for an EmailSender service
/// </summary>
public interface IEmailSender
{
    bool SendEmail(Message message);
    Task<bool> SendEmailAsync(Message message);
}
