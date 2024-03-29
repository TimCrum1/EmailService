﻿using MimeKit;
using Microsoft.AspNetCore.Http;

namespace EmailClient.EmailService;

/// <summary>
/// Message object for sending emails across Smtp
/// </summary>
public class Message
{
    public List<MailboxAddress> To { get; set; }
    public List<MailboxAddress> Bcc { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }

    public IFormFileCollection Attachments { get; set; }


    public Message(IEnumerable<string> to, IEnumerable<string> bcc, string subject, string content, IFormFileCollection attachments)
    {
        To = new();
        Bcc = new();

        To.AddRange(to.Select(add => new MailboxAddress(add, add)));
        Bcc.AddRange(to.Select(add => new MailboxAddress(add, add)));
        Subject = subject;
        Content = content;
        Attachments = attachments;
    }
}
