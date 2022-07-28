using System.ComponentModel.DataAnnotations;

namespace EmailClient.Common.EntityModels.SqlServer;

public class Email
{
    public int Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Recipients { get; set; } = string.Empty;
    [Timestamp]
    public byte[] RowVersion { get; set; }
    //not sure I need this, could use the concurrency token as sent date as there will not be any PUT methods??
    public DateTime SendDate { get; set; }
    public string Bccs { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool SentSuccessfully { get; set; }
}
