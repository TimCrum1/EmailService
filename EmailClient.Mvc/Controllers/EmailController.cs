using Microsoft.AspNetCore.Mvc;
using EmailClient.EmailService;
using EmailClient.Mvc.Models;
using System.Net.Http.Headers;

namespace EmailClient.Mvc.Controllers;

/// <summary>
/// Email controller for sending emails synchronously and asynchronously
/// </summary>
public class EmailController : Controller
{
    //logger to log errors to console (or potentially event log db)
    private readonly ILogger<EmailController> _logger;
    //HttpClientFactory to be injected from DI container
    private  IHttpClientFactory clientFactory;

    public EmailController(ILogger<EmailController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        this.clientFactory = clientFactory;
    }

    /// <summary>
    /// Method to display the Homepage of the MVC application's send email feature.
    /// </summary>
    /// <returns>A view of the Homepage</returns>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Method to display the SendEmail page of the MVC application.
    /// </summary>
    /// <returns>A view of the SendEmail page.</returns>
    [HttpGet]
    public IActionResult SendEmail()
    {
        EmailViewModel viewModel = new();
        return View(viewModel);
    }

    /// <summary>
    /// Method to send the email synchronously
    /// </summary>
    /// <param name="viewModel">The EmailViewModel</param>
    /// <returns>A view of the SentSuccessfully page, if email was sent successfully.</returns>
    [HttpPost]
    public async Task<IActionResult> SendEmail(EmailViewModel viewModel)
    {
        //determine model validity
        if (!ModelState.IsValid)
        {
            return BadRequest("Model was invalid.");
        }

        //convert each email from recipients and bcc string to a List<string>, which isn't currently being used.
        List<string> toEmails = viewModel.Recipient.Split(",").ToList();
        List<string> bccEmails = viewModel.Bcc.Split(",").ToList();

        Message message = new(toEmails, bccEmails, viewModel.Subject, viewModel.Body, viewModel.Attachments);

        string uri = "https://localhost:5003/send";

        HttpClient client = clientFactory.CreateClient();

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

        HttpRequestMessage request = new(HttpMethod.Post, uri);

        long allFilesLength = viewModel.Attachments.Sum(f => f.Length);
        byte[] filesContent = new byte[allFilesLength];

        foreach (var file in viewModel.Attachments)
        {
            if (file.Length > 0)
            {
                using (MemoryStream stream = new())
                {
                    await file.CopyToAsync(stream);
                    using (StreamContent streamContent = new(stream))
                    {
                        var attachmentsContent = await streamContent.ReadAsByteArrayAsync();
                        foreach (var bit in attachmentsContent)
                        {
                            filesContent.Append(bit);
                        }
                    }
                }
            }
        }

        ByteArrayContent byteArrayContent = new(filesContent);

        //request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        //{
        //    {"recipients", viewModel.Recipient },
        //    { "bccs", viewModel.Bcc },
        //    { "subject", viewModel.Subject },
        //    { "content", viewModel.Body },
        //    { "attachments", viewModel.Attachments }
        //});

        request.Headers.Add("recipients", viewModel.Recipient);
        request.Headers.Add("bccs", viewModel.Bcc);
        request.Headers.Add("subject", viewModel.Subject);
        request.Headers.Add("content", viewModel.Body);

        request.Content = new MultipartFormDataContent
        {
            //{ new StringContent(viewModel.Recipient), "recipients" },
            //{ new StringContent(viewModel.Bcc), "bccs" },
            //{ new StringContent(viewModel.Subject), "subject" },
            //{ new StringContent(viewModel.Body), "content" },
            byteArrayContent
        };

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
