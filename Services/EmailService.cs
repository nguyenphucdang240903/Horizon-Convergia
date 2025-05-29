using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmailService
    {
        private readonly string _from = "phucdnse172283@fpt.edu.vn";
        private readonly string _password = "ageb pldq kmav qtmq";

        public async Task SendVerificationEmailAsync(string toEmail, string verificationLink)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_from, _password)
            };

            var message = new MailMessage(_from, toEmail)
            {
                Subject = "Xác minh tài khoản",
                Body = $"Vui lòng xác minh tài khoản bằng cách nhấn vào liên kết sau:\n{verificationLink}",
                IsBodyHtml = false
            };

            await client.SendMailAsync(message);
        }
    }
}
