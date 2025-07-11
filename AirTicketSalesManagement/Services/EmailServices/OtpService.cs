using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirTicketSalesManagement.Interface;

namespace AirTicketSalesManagement.Services.EmailServices
{
    public class OtpService : IOtpService
    {
        private readonly Dictionary<string, (string Code, DateTime ExpireAt)> _otps = new();

        public string GenerateOtp(string key)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            _otps[key] = (otp, DateTime.Now.AddMinutes(3)); 

            return otp;
        }

        public bool VerifyOtp(string key, string otp)
        {
            if (_otps.TryGetValue(key, out var stored))
            {
                if (stored.ExpireAt >= DateTime.Now && stored.Code == otp)
                {
                    _otps.Remove(key); 
                    return true;
                }
            }
            return false;
        }
    }

}
