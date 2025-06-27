using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Interface
{
    public interface IPasswordResetService
    {
        Task RequestResetAsync(string email);

        Task<bool> VerifyResetCodeAsync(string email, string code);

        Task<bool> ResetPasswordAsync(string email, string newPassword);
    }

}
