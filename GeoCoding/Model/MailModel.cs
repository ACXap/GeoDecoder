using GeoCoding.MailService;
using System;
using System.Threading.Tasks;

namespace GeoCoding.Model
{
    public class MailModel
    {
        private readonly NotificationSettings _notificationSettings;
        private readonly IMailSend _mailServise;
        public MailModel(NotificationSettings set)
        {
            _notificationSettings = set;
            _mailServise = new ExchangeMail(_notificationSettings.MailSender);
        }

        public void SendEmailGeoFinish(Action<string> callbackMessage, string message, string[] files)
        {
            var rec = _notificationSettings.RecipientsResultFile.Split(';');

            var subject = "Геокодирование завершено";

            Task.Factory.StartNew(() =>
            {
                
                var res = _mailServise.Send(rec, message, subject, files);

                if (res.Successfully)
                {
                    callbackMessage("Сообщение отправлено успешно");
                }
                else
                {
                    callbackMessage("Отправка сообщения завершилось ошибкой: " + res.Error?.Message);
                }
            });
        }

        public void TestEmail(Action<string> callbackMessage)
        {
            var res = _mailServise.SendTest(_notificationSettings.MailSender);
            if (res.Successfully)
            {
                callbackMessage("Проверка работоспособности почты успешна!");
            }
            else
            {
                callbackMessage(res.Error?.Message);
            }
        }
    }
}