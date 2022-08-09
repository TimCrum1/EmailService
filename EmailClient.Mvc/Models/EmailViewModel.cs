namespace EmailClient.Mvc.Models;

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
