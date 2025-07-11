using AirTicketSalesManagement.Interface;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Services
{
    public class SendEmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;
        
        public SendEmailService()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            _smtp = configuration.GetSection("Smtp").Get<SmtpSettings>() ?? throw new InvalidOperationException("SMTP settings not found in configuration.");
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var fromAddress = new MailAddress(_smtp.User, "Air Ticket Support");
            var toAddress = new MailAddress(to);
            using var smtp = new SmtpClient
            {
                Host = _smtp.Host,
                Port = _smtp.Port,
                EnableSsl = _smtp.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtp.User, _smtp.Password)
            };
            using var msg = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            try
            {
                await smtp.SendMailAsync(msg);
                Debug.Print("[INFO] Gửi email thành công.");
                return;
            }
            catch (SmtpException ex)
            {
                Debug.Print($"[LỖI SMTP] Không gửi được email: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Debug.Print($"[LỖI] Lỗi không xác định: {ex.Message}");
                return;
            }
        }
    }
}
