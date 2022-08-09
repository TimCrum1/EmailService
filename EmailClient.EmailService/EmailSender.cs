using MailKit.Net.Smtp;
using MimeKit;

namespace EmailClient.EmailService;

/// <summary>
/// EmailSender service for sending email messages across SMTP
/// </summary>
public class EmailSender : IEmailSender
{
    //the email config to be injected from DI container
    private EmailConfiguration emailConfig;


    public EmailSender(EmailConfiguration emailConfig)
    {
        this.emailConfig = emailConfig;
    }

    /// <summary>
    /// Send an email message
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns>True if sent successfully</returns>
    public bool SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        return Send(emailMessage);
    }

    /// <summary>
    /// Send an email message asynchronously.
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns>True if sent successfully</returns>
    public async Task<bool> SendEmailAsync(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        return await SendAsync(emailMessage);
    }

    /// <summary>
    /// Private helper method to create a MimeMessage from the Message object model.
    /// </summary>
    /// <param name="message">The message object model</param>
    /// <returns>The MimeMessage object to be sent</returns>
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

        if (message.Attachments != null && message.Attachments.Any())
        {
            byte[] filesContent;
            foreach (var attachment in message.Attachments)
            {
                using (var stream = new MemoryStream())
                {
                    attachment.CopyTo(stream);
                    filesContent = stream.ToArray();
                    stream.Close();
                }
                bodyBuilder.Attachments.Add(attachment.FileName, filesContent, ContentType.Parse(attachment.ContentType));
            }
        }
        emailMessage.Body = bodyBuilder.ToMessageBody();

        return emailMessage;
    }

    /// <summary>
    /// Private helper method to Send the MimeMessage that is created from the Public Send method
    /// </summary>
    /// <param name="emailMessage">The MimeMessage created in Public Send method</param>
    /// <returns>True if sent successfully</returns>
    private bool Send(MimeMessage emailMessage)
    {
        //counter to attempt to send message three times
        int tryCounter = 0;
        bool successful = false;

        //while counter less than three and email hasn't been sent successfully
        while (tryCounter < 3 && !successful)
        {
            //using scope for SmtpClient
            using (var client = new SmtpClient())
            {
                try
                {
                    //attempt to connect the SmtpClient to the SmtpHost and port provided by the email configuration object
                    client.Connect(emailConfig.SmtpHost, emailConfig.Port, true);
                    //remove OAuth2 from the authentication mechanism of the SmtpClient
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    //authenticate the sender email's username and password injected from AppSettings.json into the email config option
                    client.Authenticate(emailConfig.Username, emailConfig.Password);
                    //send the message
                    client.Send(emailMessage);
                    successful = true;
                }
                catch (Exception ex)
                {
                    //exception should only be thrown if the email is not sent successfully
                    //increment counter and attempt again
                    tryCounter++;
                }
                finally
                {
                    //after each attempt, disconnect and dispose of the SmtpClient
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        //return send success status
        return successful;
    }

    /// <summary>
    /// Private helper method to asynchronously send the MimeMessage that is created from the Public SendAsync method
    /// </summary>
    /// <param name="emailMessage">The MimeMessage created in the public SendAsync method</param>
    /// <returns>True if sent successfully</returns>
    private async Task<bool> SendAsync(MimeMessage emailMessage)
    {
        //same set up as the synchronous method, only using asynchronous calls.
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
