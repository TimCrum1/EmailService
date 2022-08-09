using EmailClient.Common.EntityModels.SqlServer;
using EmailClient.EmailService;
using EmailClient.WebApi.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailClient.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailServiceController : Controller
{
    private EmailConfiguration emailConfiguration;
    private IEmailSender emailSender;
    private readonly EmailRepo emailRepo;

    public EmailServiceController(EmailConfiguration emailConfiguration, EmailRepo emailRepo)
    {
        this.emailConfiguration = emailConfiguration;
        emailSender = new EmailSender(emailConfiguration);
        this.emailRepo = emailRepo;
    }


    // POST: api/emailservice 
    [HttpPost]
    [Route("/send")]
    [ProducesResponseType(200, Type = typeof(Message))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SendEmail(string recipients, string bccs, string subject, string content, IFormFileCollection attachments)
    {
        List<string> recipientsList = recipients.Split(",").ToList();
        List<string> bccList = bccs.Split(",").ToList();

        //if (attachments == null || !attachments.Any())
        //{
        //    attachments = new FormFileCollection();
        //}

        Message message = new Message(recipientsList, bccList, subject, content, attachments);

        bool success = await emailSender.SendEmailAsync(message);

        Email email = new()
        {
            Sender = emailConfiguration.MailFrom,
            Subject = subject,
            Message = content,
            Recipients = recipients,
            Bccs = bccs,
            SendDate = DateTime.Now,
        };

        if (success)
        {
            email.SentSuccessfully = true;
            emailRepo.Add(email);
            return Ok();
        }
        else
        {
            email.SentSuccessfully = false;
            emailRepo.Add(email);
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("/sendtest")]
    public async Task<bool> SendTestEmail(string recipient)
    {
        List<string> recipients = new();
        List<string> bccs = new();
        FormFileCollection attachments = new();

        recipients.Add(recipient);
        var message = new Message(recipients, bccs, "Test Email", "This is a test email sent to you by the EmailService service.", attachments);
        bool success = await emailSender.SendEmailAsync(message);
        Email email = new Email()
        {
            Sender = emailConfiguration.MailFrom,
            Recipients = recipient,
            Bccs = string.Empty,
            Message = message.Content,
            Subject = message.Subject,
            SendDate = DateTime.Now,
        };
        if (success)
        {
            email.SentSuccessfully = true;
            emailRepo.Add(email);
        }
        else
        {
            email.SentSuccessfully = false;
            emailRepo.Add(email);
        }
        return success;
    }

}
