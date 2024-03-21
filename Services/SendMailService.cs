using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
namespace webapi.Services
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string? DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class SendMailService : ISendMailService
    {
        private readonly MailSettings _settings;

        public SendMailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var Message = new MimeMessage();
            Message.Sender = new MailboxAddress(_settings.DisplayName, _settings.Mail);
            Message.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Mail));
            Message.To.Add(MailboxAddress.Parse(email));
            Message.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };
            Message.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_settings.Mail, _settings.Password);
                    await smtp.SendAsync(Message);
                }
                catch (Exception ex)
                {
                    Directory.CreateDirectory("MailsSave");
                    var emailsavefile = string.Format(@"MailsSave/{0}.txt", email + Guid.NewGuid());
                    await Message.WriteToAsync(emailsavefile);
                    await File.AppendAllTextAsync(emailsavefile, ex.Message);
                }
                await smtp.DisconnectAsync(true);
            }

        }

        public async Task SendEmailAsync(List<string> email, string subject, string htmlMessage, List<string> imagePath)
        {
            var Message = new MimeMessage();
            Message.Sender = new MailboxAddress(_settings.DisplayName, _settings.Mail);
            Message.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Mail));
            foreach(var item in email)
            {
                Message.To.Add(MailboxAddress.Parse(item));
            }
            Message.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };
            var i = 0;
            foreach (var path in imagePath)
            {
                var image = builder.LinkedResources.Add(path);
                image.ContentId = MimeUtils.GenerateMessageId();
                var str = $"Logo{i}.jpg";
                builder.HtmlBody = builder.HtmlBody.Replace(str, image.ContentId);
                i++;
            }
            Message.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_settings.Mail, _settings.Password);
                    await smtp.SendAsync(Message);
                }
                catch (Exception ex)
                {
                    Directory.CreateDirectory("MailsSave");
                    foreach(var item in email)
                    {
                        var emailsavefile = string.Format(@"MailsSave/{0}.txt", item + Guid.NewGuid());
                        await Message.WriteToAsync(emailsavefile);
                        await File.AppendAllTextAsync(emailsavefile, ex.Message);
                    }
                }
                await smtp.DisconnectAsync(true);
            }

        }
    }
}
