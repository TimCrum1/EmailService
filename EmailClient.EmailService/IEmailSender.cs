using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.EmailService;

public interface IEmailSender
{
    bool SendEmail(Message message);
    Task<bool> SendEmailAsync(Message message);
}
