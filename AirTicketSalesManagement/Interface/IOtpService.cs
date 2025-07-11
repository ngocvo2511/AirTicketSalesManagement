using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Interface
{
    public interface IOtpService
    {
        string GenerateOtp(string key);         
        bool VerifyOtp(string key, string otp);
    }
}
