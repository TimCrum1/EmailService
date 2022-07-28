using Microsoft.AspNetCore.Mvc;
using EmailClient.WebApi;
using EmailClient.EmailService;
using EmailClient.Mvc.Models;
using MimeKit;
using Newtonsoft.Json;
using System.Text;

namespace EmailClient.Mvc.Controllers;

public class EmailController : Controller
{
    private readonly ILogger<EmailController> _logger;
    private  IHttpClientFactory clientFactory;

    public EmailController(ILogger<EmailController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        this.clientFactory = clientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult SendEmail()
    {
        EmailViewModel viewModel = new();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(EmailViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Model was invalid.");
        }

        //List<string> toEmails = viewModel.Recipient.Split(",").ToList();
        //List<string> bccEmails = viewModel.Bcc.Split(",").ToList();

        //Message message = new(toEmails, bccEmails, viewModel.Subject, viewModel.Body); //, viewModel.Attachments);
        
        string uri = "/send";

        HttpClient client = clientFactory.CreateClient(
            name: "EmailClient.WebApi");

        HttpRequestMessage request = new(HttpMethod.Post, uri);

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "recipients", viewModel.Recipient },
            { "bccs", viewModel.Bcc },
            { "subject", viewModel.Subject },
            { "content", viewModel.Body }
        });

        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email sent successfully");
            return View("EmailSent", viewModel);

        }
        else
        {
            _logger.LogError($"Status Code: {response.StatusCode} Reason: {response.ReasonPhrase}");
            return BadRequest($"Status Code: {response.StatusCode} Reason {response.ReasonPhrase}");
        }

    }
}
