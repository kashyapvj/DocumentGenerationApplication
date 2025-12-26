using System.Net;
using System.Net.Mail;

namespace DocumentGenerationApplication.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {

            try 
            {
                var message = new MailMessage();
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress(_config["Email:From"]);

                using var smtp = new SmtpClient(_config["Email:Host"])
                {
                    Port = int.Parse(_config["Email:Port"]),
                    Credentials = new NetworkCredential(
                        _config["Email:Username"], _config["Email:Password"]),

                    EnableSsl = true
                };

                await smtp.SendMailAsync(message);
            }
            catch(Exception ex)
            {
                var errMessage = ex.Message;
                throw new ApplicationException("Error in SendEmailAsync: " + ex.Message, ex);
            }
        
        }
    }

}
