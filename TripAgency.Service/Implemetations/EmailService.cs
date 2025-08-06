using MailKit.Net.Smtp;
using MimeKit;
using TripAgency.Data.Helping;
using TripAgency.Service.Abstracts;

namespace TripAgency.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        #region Failds

        #endregion
        #region Constructore(s)
        public EmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }


        #endregion

        #region Handler Functions
        public async Task<string> SendEmailAsync(string email, string Message, string? reason)
        {
            try
            {
               // sending the Message of passwordResetLink
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, true);
                    client.Authenticate(_emailSettings.FromEmail, _emailSettings.Password);
                    var bodybuilder = new BodyBuilder
                    {
                        HtmlBody = $"{Message}",
                        TextBody = "wellcome",
                    };
                    var message = new MimeMessage
                    {
                        Body = bodybuilder.ToMessageBody()
                    };
                    message.From.Add(new MailboxAddress("trip agency", _emailSettings.FromEmail));
                    message.To.Add(new MailboxAddress("testing", email));
                    message.Subject = reason == null ? "No Submitted" : reason;
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                //end of sending email

                return "Success";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

