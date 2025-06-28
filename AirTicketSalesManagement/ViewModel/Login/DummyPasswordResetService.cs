using AirTicketSalesManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Diagnostics;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public class DummyPasswordResetService : IPasswordResetService
    {
        private Dictionary<string, string> _codeStorage = new();
        private readonly SmtpSettings _smtp;
        public DummyPasswordResetService()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            _smtp = configuration.GetSection("Smtp").Get<SmtpSettings>() ?? throw new InvalidOperationException("SMTP settings not found in configuration.");
        }

        public async Task RequestResetAsync(string email)
        {
            string code = GenerateCode();
            _codeStorage[email] = code;

            var fromAddress = new MailAddress(_smtp.User, "Air Ticket Support");
            var toAddress = new MailAddress(email);
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
                Subject = "Mã xác nhận đặt lại mật khẩu",
                Body = $"Mã xác nhận của bạn là: {code}"
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

        public Task<bool> VerifyResetCodeAsync(string email, string code)
        {
            return Task.FromResult(_codeStorage.TryGetValue(email, out var storedCode) && storedCode == code);
        }

        public Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            // Cập nhật mật khẩu trong CSDL ở đây
            Console.WriteLine($"[DEV] Cập nhật mật khẩu mới cho {email}: {newPassword}");
            return Task.FromResult(true);
        }

        private string GenerateCode()
        {
            Random rnd = new();
            return rnd.Next(100000, 999999).ToString();
        }
    }

}
