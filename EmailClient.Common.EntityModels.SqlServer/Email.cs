using System.ComponentModel.DataAnnotations;

namespace EmailClient.Common.EntityModels.SqlServer;

/// <summary>
/// DB Entity model for an Email object to be stored in a Microsoft SQL Server DB specifically.
/// </summary>
public class Email
{
    public int Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Recipients { get; set; } = string.Empty;
    [Timestamp]
    public byte[] RowVersion { get; set; }
    public DateTime SendDate { get; set; }
    public string Bccs { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool SentSuccessfully { get; set; }
}
