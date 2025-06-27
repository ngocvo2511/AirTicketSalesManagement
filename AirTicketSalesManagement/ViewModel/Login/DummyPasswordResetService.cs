using AirTicketSalesManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public class DummyPasswordResetService : IPasswordResetService
    {
        private Dictionary<string, string> _codeStorage = new();

        public Task RequestResetAsync(string email)
        {
            string code = GenerateCode(); // ví dụ "123456"
            _codeStorage[email] = code;

            // Gửi email giả (thực tế bạn dùng SMTP hoặc dịch vụ như SendGrid)
            Console.WriteLine($"[DEV] Mã xác nhận gửi đến {email}: {code}");

            return Task.CompletedTask;
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
