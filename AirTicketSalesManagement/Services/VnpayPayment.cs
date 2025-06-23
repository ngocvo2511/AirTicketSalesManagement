using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace AirTicketSalesManagement.Services
{
    public class VnpayPayment
    {
        private string tmnCode = "B2G07RMI";
        private string hashSecret = "GWB7ENE65XM4QCX0SD1GC8UWTUH0PJS2";
        private string baseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private string callbackUrl = "https://localhost:1234/api/Vnpay/Callback";

        private readonly IVnpay vnpay;

        public VnpayPayment()
        {
            // Khởi tạo giá trị cho _tmnCode, _hashSecret, _baseUrl, _callbackUrl tại đây.
            vnpay = new Vnpay();
            vnpay.Initialize(tmnCode, hashSecret, baseUrl, callbackUrl);
        }

        public string CreatePaymentUrl(double amount, string description, long Id)
        {
            var request = new PaymentRequest
            {
                PaymentId = Id,
                Money = amount,
                Description = description,
                IpAddress = GetLocalIPAddress(),
                BankCode = BankCode.ANY,
                CreatedDate = DateTime.Now,
                Currency = Currency.VND,
                Language = DisplayLanguage.Vietnamese
            };

            return vnpay.GetPaymentUrl(request);
        }

        public string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}
