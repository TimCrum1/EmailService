namespace EmailClient.EmailService;

/// <summary>
/// Email configuration object
/// </summary>
public class EmailConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string MailTo { get; set; } = string.Empty;
    public string Bcc { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string SmtpHost { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string MailFrom { get; set; } = string.Empty;
    public int Port { get; set; }
}
