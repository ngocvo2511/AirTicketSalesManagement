using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using System.Diagnostics;

namespace AirTicketSalesManagement.Helper
{
    public static class PaymentDebugHelper
    {
        public static void LogUserSession()
        {
            Debug.WriteLine("=== USER SESSION DEBUG ===");
            Debug.WriteLine($"AccountId: {UserSession.Current.AccountId}");
            Debug.WriteLine($"CustomerId: {UserSession.Current.CustomerId}");
            Debug.WriteLine($"StaffId: {UserSession.Current.StaffId}");
            Debug.WriteLine($"CustomerName: {UserSession.Current.CustomerName}");
            Debug.WriteLine($"Email: {UserSession.Current.Email}");
            Debug.WriteLine($"isStaff: {UserSession.Current.isStaff}");
            Debug.WriteLine("==========================");
        }

        public static void LogRecentBookings(int? customerId = null, int? staffId = null)
        {
            Debug.WriteLine("=== RECENT BOOKINGS DEBUG ===");
            
            using (var context = new AirTicketDbContext())
            {
                var query = context.Datves.AsQueryable();
                
                if (customerId.HasValue)
                {
                    query = query.Where(dv => dv.MaKh == customerId.Value);
                    Debug.WriteLine($"Searching for customer bookings with MaKh: {customerId.Value}");
                }
                else if (staffId.HasValue)
                {
                    query = query.Where(dv => dv.MaNv == staffId.Value);
                    Debug.WriteLine($"Searching for staff bookings with MaNv: {staffId.Value}");
                }
                
                var recentBookings = query
                    .Where(dv => dv.ThoiGianDv >= System.DateTime.Now.AddMinutes(-30))
                    .OrderByDescending(dv => dv.ThoiGianDv)
                    .Take(5)
                    .ToList();

                Debug.WriteLine($"Found {recentBookings.Count} recent bookings:");
                
                foreach (var booking in recentBookings)
                {
                    Debug.WriteLine($"  - MaDv: {booking.MaDv}, MaKh: {booking.MaKh}, MaNv: {booking.MaNv}, Status: {booking.TtdatVe}, Time: {booking.ThoiGianDv}");
                }
            }
            
            Debug.WriteLine("=============================");
        }

        public static void LogPaymentStatus(int bookingId)
        {
            Debug.WriteLine($"=== PAYMENT STATUS DEBUG for Booking {bookingId} ===");
            
            using (var context = new AirTicketDbContext())
            {
                var booking = context.Datves.FirstOrDefault(dv => dv.MaDv == bookingId);
                
                if (booking != null)
                {
                    Debug.WriteLine($"Booking found:");
                    Debug.WriteLine($"  - MaDv: {booking.MaDv}");
                    Debug.WriteLine($"  - MaKh: {booking.MaKh}");
                    Debug.WriteLine($"  - MaNv: {booking.MaNv}");
                    Debug.WriteLine($"  - Status: {booking.TtdatVe}");
                    Debug.WriteLine($"  - Total Amount: {booking.TongTienTt}");
                    Debug.WriteLine($"  - Booking Time: {booking.ThoiGianDv}");
                    
                    // Log passengers
                    var passengers = context.Ctdvs.Where(ct => ct.MaDv == bookingId).ToList();
                    Debug.WriteLine($"  - Number of passengers: {passengers.Count}");
                    
                    foreach (var passenger in passengers)
                    {
                        Debug.WriteLine($"    * {passenger.HoTenHk} - {passenger.Cccd}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Booking with ID {bookingId} not found");
                }
            }
            
            Debug.WriteLine("===============================================");
        }

        public static void ValidateUserSessionForPayment()
        {
            Debug.WriteLine("=== VALIDATING USER SESSION FOR PAYMENT ===");
            
            if (UserSession.Current == null)
            {
                Debug.WriteLine("ERROR: UserSession.Current is null");
                return;
            }

            if (UserSession.Current.isStaff)
            {
                if (!UserSession.Current.StaffId.HasValue)
                {
                    Debug.WriteLine("ERROR: Staff user but StaffId is null");
                }
                else
                {
                    Debug.WriteLine($"OK: Staff user with StaffId: {UserSession.Current.StaffId.Value}");
                }
            }
            else
            {
                if (!UserSession.Current.CustomerId.HasValue)
                {
                    Debug.WriteLine("ERROR: Customer user but CustomerId is null");
                }
                else
                {
                    Debug.WriteLine($"OK: Customer user with CustomerId: {UserSession.Current.CustomerId.Value}");
                }
            }
            
            Debug.WriteLine("=============================================");
        }
    }
} 