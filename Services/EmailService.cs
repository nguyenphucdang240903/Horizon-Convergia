using Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Services
{
    public class EmailService : IEmailService
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
            // HTML content cho email xác minh
            string htmlBody = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Xác minh tài khoản - Horizon Convergia</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #ffffff, #f8fafc);
            padding: 24px;
            color: #334155;
        }}
        .email-container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 8px 30px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background: linear-gradient(135deg, #4285f4, #9c27b0);
            padding: 40px 30px;
            text-align: center;
            color: white;
        }}
        .logo {{
            width: 72px;
            margin-bottom: 16px;
        }}
        .email-title {{
            font-size: 28px;
            font-weight: bold;
        }}
        .decorative-bar {{
            height: 5px;
            background: linear-gradient(to right, #ffc107, #4285f4, #9c27b0);
        }}
        .email-body {{
            padding: 32px;
        }}
        .info-box {{
            background: #fef3cd;
            border-left: 4px solid #ffd60a;
            padding: 16px;
            margin: 24px 0;
            border-radius: 6px;
        }}
        .cta-button {{
            display: inline-block;
            padding: 14px 30px;
            background: linear-gradient(135deg, #ffc107, #ff8f00);
            color: white;
            font-weight: bold;
            text-decoration: none;
            border-radius: 8px;
            margin: 24px auto;
            text-align: center;
        }}
        .cta-button:hover {{
            background: linear-gradient(135deg, #e0a800, #ff6f00);
        }}
        .alt-link {{
            font-size: 14px;
            word-break: break-word;
            background: #f8fafc;
            padding: 16px;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
            margin-top: 24px;
        }}
        .footer {{
            background: #f8fafc;
            padding: 20px 32px;
            text-align: center;
            font-size: 14px;
            color: #64748b;
        }}
        .login-link {{
            display: inline-block;
            margin-top: 12px;
            padding: 10px 22px;
            background: #4285f4;
            color: #fff;
            border-radius: 6px;
            text-decoration: none;
            font-weight: 500;
        }}
        .login-link:hover {{
            background: #3367d6;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <img src='https://www.horizonconvergia.click/assets/logo-Cw2ulTmz.png' alt='Horizon Convergia Logo' class='logo' />
            <div class='email-title'>Xác minh tài khoản</div>
            <div>Hoàn tất đăng ký của bạn</div>
        </div>
        <div class='decorative-bar'></div>
        <div class='email-body'>
            <p>Chào bạn,</p>
            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>Horizon Convergia</strong>.</p>
            <div class='info-box'>
                <p>Hãy nhấn vào nút bên dưới để xác minh email và kích hoạt tài khoản của bạn.</p>
            </div>
            <div style='text-align: center;'>
                <a href='{verificationLink}' class='cta-button'>Xác minh tài khoản</a>
            </div>
            <p style='margin-top: 24px;'>Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.</p>
        </div>
        <div class='footer'>
            <p>Trân trọng,<br><strong>Horizon Convergia</strong> | FPT University</p>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>";



            var message = new MailMessage(_from, toEmail)
            {
                Subject = "Xác minh tài khoản",
                Body = htmlBody,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_from, _password)
            };
            string htmlBody = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Xác minh tài khoản - Horizon Convergia</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #ffffff, #f8fafc);
            padding: 24px;
            color: #334155;
        }}
        .email-container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 8px 30px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background: linear-gradient(135deg, #4285f4, #9c27b0);
            padding: 40px 30px;
            text-align: center;
            color: white;
        }}
        .logo {{
            width: 72px;
            margin-bottom: 16px;
        }}
        .email-title {{
            font-size: 28px;
            font-weight: bold;
        }}
        .decorative-bar {{
            height: 5px;
            background: linear-gradient(to right, #ffc107, #4285f4, #9c27b0);
        }}
        .email-body {{
            padding: 32px;
        }}
        .info-box {{
            background: #fef3cd;
            border-left: 4px solid #ffd60a;
            padding: 16px;
            margin: 24px 0;
            border-radius: 6px;
        }}
        .cta-button {{
            display: inline-block;
            padding: 14px 30px;
            background: linear-gradient(135deg, #ffc107, #ff8f00);
            color: white;
            font-weight: bold;
            text-decoration: none;
            border-radius: 8px;
            margin: 24px auto;
            text-align: center;
        }}
        .cta-button:hover {{
            background: linear-gradient(135deg, #e0a800, #ff6f00);
        }}
        .alt-link {{
            font-size: 14px;
            word-break: break-word;
            background: #f8fafc;
            padding: 16px;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
            margin-top: 24px;
        }}
        .footer {{
            background: #f8fafc;
            padding: 20px 32px;
            text-align: center;
            font-size: 14px;
            color: #64748b;
        }}
        .login-link {{
            display: inline-block;
            margin-top: 12px;
            padding: 10px 22px;
            background: #4285f4;
            color: #fff;
            border-radius: 6px;
            text-decoration: none;
            font-weight: 500;
        }}
        .login-link:hover {{
            background: #3367d6;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <img src='https://www.horizonconvergia.click/assets/logo-Cw2ulTmz.png' alt='Horizon Convergia Logo' class='logo' />
            <div class='email-title'>Yêu cầu đặt lại mật khẩu</div>
            <div>Khôi phục tài khoản của bạn</div>
        </div>
        <div class='decorative-bar'></div>
        <div class='email-body'>
            <p>Chào bạn,</p>
            <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu từ bạn tại <strong>Horizon Convergia</strong>.</p>
            <div class='info-box'>
                <p>Hãy nhấn vào nút bên dưới để đặt lại mật khẩu. Liên kết này sẽ hết hạn sau <strong>1 giờ</strong>.</p>
            </div>
            <div style='text-align: center;'>
                <a href='{resetLink}' class='cta-button'>Đặt lại mật khẩu</a>
            </div>
            <p style='margin-top: 24px;'>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
        </div>
        <div class='footer'>
            <p>Trân trọng,<br><strong>Horizon Convergia</strong> | FPT University</p>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>";

            var message = new MailMessage(_from, toEmail)
            {
                Subject = "Đặt lại mật khẩu - Horizon Convergia",
                Body = htmlBody,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
        public async Task SendPaymentEmailAsync(string toEmail, string verificationLink)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_from, _password)
            };

            var message = new MailMessage(_from, toEmail)
            {
                Subject = "Thanh toán phí dịch vụ",
                Body = $"Để thanh toán vui lòng nhấp vào liên kết sau:\n{verificationLink}",
                IsBodyHtml = false
            };

            await client.SendMailAsync(message);
        }
    }

}
