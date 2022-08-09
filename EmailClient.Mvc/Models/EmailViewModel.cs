namespace EmailClient.Mvc.Models;

/// <summary>
/// ViewModel for the Email that is being sent.
/// </summary>
public class EmailViewModel
{
    public string Recipient { get; set; } = string.Empty;
    public string Bcc { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public IFormFileCollection Attachments { get; set; }

    public EmailViewModel()
    {
        Attachments = new FormFileCollection();
    }
}
