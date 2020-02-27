using GeoCoding.Entities;
using System.Collections.Generic;

namespace GeoCoding.MailService
{
    public interface IMailSend
    {
        EntityResult<bool> Send(IEnumerable<string> recipients, string message, string subject, IEnumerable<string> files);

        EntityResult<bool> SendTest(string senderMail);
    }
}