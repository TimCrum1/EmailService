using EmailClient.Common.EntityModels.SqlServer;
using EmailClient.EmailService;
using EmailClient.WebApi.DAL;
using Microsoft.AspNetCore.Mvc;

namespace EmailClient.WebApi.Controllers;

/// <summary>
/// Controller for sending Emails using the EmailClient.EmailService class library
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmailServiceController : Controller
{
    //email config object holding SmtpHost and PortNumber as well as the sending email's username and password (injected from DI container)
    private EmailConfiguration emailConfiguration;
    //EmailClient.EmailService object
    private IEmailSender emailSender;
    //DAL for storing send attempts to database (whether they are successful or not)
    private readonly EmailRepo emailRepo;


    public EmailServiceController(EmailConfiguration emailConfiguration, EmailRepo emailRepo)
    {
        this.emailConfiguration = emailConfiguration;
        emailSender = new EmailSender(emailConfiguration);
        this.emailRepo = emailRepo;
    }

    /// <summary>
    /// Send an email synchronously
    /// </summary>
    /// <param name="recipients">The email address of the recipients, separated by a comma</param>
    /// <param name="bccs">The email address of anyone who is bcc'd, separated by a comma</param>
    /// <param name="subject">The subject line of the email</param>
    /// <param name="content">The message body or content of the email</param>
    /// <param name="attachments">Any file attachments to be sent with the email</param>
    /// <returns></returns>
    // POST: api/emailservice 
    [HttpPost]
    [Route("/send")]
    [ProducesResponseType(200, Type = typeof(Message))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SendEmail(string recipients, string bccs, string subject, string content, IFormFileCollection attachments)
    {
        //create List<string> for recipients and bccs
        List<string> recipientsList = recipients.Split(",").ToList();
        List<string> bccList = bccs.Split(",").ToList();

        //Create EmailService Message object from parameters
        Message message = new Message(recipientsList, bccList, subject, content, attachments);

        //Send the email and record success
        bool success = await emailSender.SendEmailAsync(message);

        //create local DB model for email to be stored in DB (all except for attachments)
        Email email = new()
        {
            Sender = emailConfiguration.MailFrom,
            Subject = subject,
            Message = content,
            Recipients = recipients,
            Bccs = bccs,
            SendDate = DateTime.Now,
        };

        //if successful save email and return 200 status code
        if (success)
        {
            email.SentSuccessfully = true;
            emailRepo.Add(email);
            return Ok();
        }
        //else save email and return 400 bad request status code
        else
        {
            email.SentSuccessfully = false;
            emailRepo.Add(email);
            return BadRequest();
        }
    }

    /// <summary>
    /// Send a test email using the EmailClient.EmailService class library
    /// </summary>
    /// <param name="recipient">The recipient of the email</param>
    /// <returns></returns>
    [HttpPost]
    [Route("/sendtest")]
    public async Task<bool> SendTestEmail(string recipient)
    {
        List<string> recipients = new();
        List<string> bccs = new();
        FormFileCollection attachments = new();

        //add each recipient to the Message
        foreach(string rec in recipient.Split(","))
        {
            recipients.Add(rec);
        }

        //create the Message object
        var message = new Message(recipients, bccs, "Test Email", "This is a test email sent to you by the EmailService service.", attachments);
        //send email and record success
        bool success = await emailSender.SendEmailAsync(message);
        //create DB entity to store the send attempt
        Email email = new Email()
        {
            Sender = emailConfiguration.MailFrom,
            Recipients = recipient,
            Bccs = string.Empty,
            Message = message.Content,
            Subject = message.Subject,
            SendDate = DateTime.Now,
        };

        //record success to db entity
        if (success)
        {
            email.SentSuccessfully = true;
        }
        else
        {
            email.SentSuccessfully = false;
            
        }
        //save send attempt to db
        emailRepo.Add(email);
        
        //return if email was sent successfully
        return success;
    }

}
