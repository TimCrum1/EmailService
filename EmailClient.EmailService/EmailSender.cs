using MailKit.Net.Smtp;
using MimeKit;

namespace EmailClient.EmailService;

public class EmailSender : IEmailSender
{
    private EmailConfiguration emailConfig;

    public EmailSender(EmailConfiguration emailConfig)
    {
        this.emailConfig = emailConfig;
    }

    public bool SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        return Send(emailMessage);
    }

    public async Task<bool> SendEmailAsync(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        return await SendAsync(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(emailConfig.Name, emailConfig.MailFrom));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        var bodyBuilder = new BodyBuilder
        {
            TextBody = message.Content
        };

        //if (message.Attachments != null && message.Attachments.Any())
        //{
        //    byte[] filesContent;
        //    foreach (var attachment in message.Attachments)
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            attachment.CopyTo(stream);
        //            filesContent = stream.ToArray();
        //        }
        //        bodyBuilder.Attachments.Add(attachment.FileName, filesContent, ContentType.Parse(attachment.ContentType));
        //    }
        //}
        emailMessage.Body = bodyBuilder.ToMessageBody();

        return emailMessage;
    }

    private bool Send(MimeMessage emailMessage)
    {
        int tryCounter = 0;
        bool successful = false;

        while (tryCounter < 3 && !successful)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(emailConfig.SmtpHost, emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailConfig.Username, emailConfig.Password);

                    client.Send(emailMessage);
                    successful = true;
                }
                catch (Exception ex)
                {
                    tryCounter++;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        return successful;
    }

    private async Task<bool> SendAsync(MimeMessage emailMessage)
    {
        int tryCounter = 0;
        bool successful = false;

        while (tryCounter < 3 && !successful)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(emailConfig.SmtpHost, emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(emailConfig.Username, emailConfig.Password);

                    await client.SendAsync(emailMessage);
                    successful = true;
                }
                catch
                {
                    //email was not sent successfully
                    tryCounter++;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
        return successful;
    }
}
