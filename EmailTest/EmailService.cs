using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Text;
using MailKit.Net.Smtp;

namespace EmailTest
{
    public class EmailService : IEmailService
    {
        private readonly SmtpConfig _smtpConfig;
        private readonly UserInterfaceConfig _userInterfaceConfig;
        private readonly MailTemplateConfig _mailTemplateConfig;

        public EmailService(IOptions<SmtpConfig> smtpConfig, IOptions<UserInterfaceConfig> userInterfaceConfig, IOptions<MailTemplateConfig> mailTemplateConfig)
        {
            _smtpConfig = smtpConfig.Value;
            _userInterfaceConfig = userInterfaceConfig.Value;
            _mailTemplateConfig = mailTemplateConfig.Value;
        }

        public async Task<bool> SendChangePassword(string to, string token)
        {
            try
            {
                if (!File.Exists(_mailTemplateConfig.ForgetPasswordTemplateUrlForAdmin))
                    throw new Exception();

                string html = File.ReadAllText(_mailTemplateConfig.ForgetPasswordTemplateUrlForAdmin);

                StringBuilder body = new StringBuilder(html);

                body.Replace("{{token}}", token);
                body.Replace("{{urlAdmin}}", _userInterfaceConfig.UrlAdmin);
                body.Replace("{{baseUrl}}", _userInterfaceConfig.BaseUrl);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_smtpConfig.Name, _smtpConfig.Email));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = "Şifremi Unuttum";
                email.Body = new TextPart(TextFormat.Html) { Text = body.ToString() };

                await SendMail(email);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<EmailCodeDto> SendCode(string to)
        {
            try
            {
                var code = GenerateCode();
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_smtpConfig.Name, _smtpConfig.Email));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = "6 Haneli Doğrulama Kodu";
                email.Body = new TextPart(TextFormat.Html) { Text = code };
                await SendMail(email);
                return new EmailCodeDto { Email = to, Code = code, Result = true };
            }
            catch
            {
                return new EmailCodeDto { Result = false };
            }
        }

        public async Task<bool> SendGeneratePassword(string to, string password)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_smtpConfig.Name, _smtpConfig.Email));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = "Yeni Şifre";
                email.Body = new TextPart(TextFormat.Html) { Text = password };
                await SendMail(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task SendMail(MimeMessage email)
        {
            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpConfig.Username, _smtpConfig.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {

                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        private string GenerateCode()
        {
            Random generator = new Random();
            var code = generator.Next(0, 1000000).ToString("D6");
            return code;
        }
    }
}
