using GeoCoding.Entities;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding.MailService
{
    public class ExchangeMail : IMailSend
    {
        private readonly string _senderMail;
        private ExchangeService service;
        //private const string SUBJECT_MAIL_SENDT_RESULT_FILE = "Результаты геокодирования";

        public ExchangeMail(string senderMail)
        {
            _senderMail = senderMail;
        }

        public EntityResult<bool> Send(IEnumerable<string> recipients, string message, string subject, IEnumerable<string> files)
        {
            EntityResult<bool> result = new EntityResult<bool>();

            try
            {
                Connect();
                EmailMessage email = new EmailMessage(service);

                foreach (var recipient in recipients)
                {
                    email.ToRecipients.Add(new EmailAddress() { Address = recipient });
                }

                email.Subject = subject;
                email.Body = new MessageBody(BodyType.HTML, message);

                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            email.Attachments.AddFileAttachment(file);
                        };
                    }
                }

                //email.IsDeliveryReceiptRequested = true;
                //email.IsReadReceiptRequested = true;
                email.SendAndSaveCopy();

                result.Successfully = true;
                result.Entity = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<bool> SendTest(string senderMail)
        {
            return Send(new string[] { senderMail }, "Тестовое сообщение" + Environment.NewLine + "Test", "Проверка", null);
        }

        private void Connect()
        {
            service = new ExchangeService(ExchangeVersion.Exchange2007_SP1)
            {
                UseDefaultCredentials = true
            };
            service.AutodiscoverUrl(_senderMail, RedirectionUrlValidationCallback);
        }

        private bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }
    }
}