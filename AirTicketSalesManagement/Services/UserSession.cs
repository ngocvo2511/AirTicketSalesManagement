using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Services
{
    public class UserSession
    {
        public int? AccountId { get; set; }
        public int? CustomerId { get; set; }

        public int? StaffId { get; set; }
        public string CustomerName { get; set; }

        public string Email { get; set; }
        public bool isStaff { get; set; } = false;
        // các thuộc tính khác
        public int idVe { get; set; }
        public static UserSession Current { get; } = new UserSession();
    }
}
