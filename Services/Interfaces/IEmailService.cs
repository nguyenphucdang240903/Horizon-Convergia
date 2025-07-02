using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface  IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string verificationLink);
        Task SendResetPasswordEmailAsync(string toEmail, string resetLink);
        Task SendPaymentEmailAsync(string toEmail, string verificationLink);
    }
    
}
