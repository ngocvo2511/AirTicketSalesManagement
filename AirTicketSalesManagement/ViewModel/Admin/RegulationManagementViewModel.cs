using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class RegulationManagementViewModel : BaseViewModel
    {
        // Quy định hiện tại
        [ObservableProperty]
        private int maxAirports;
        [ObservableProperty]
        private int minFlightTime;
        [ObservableProperty]
        private int maxStopover;
        [ObservableProperty]
        private int minStopTime;
        [ObservableProperty]
        private int maxStopTime;
        [ObservableProperty]
        private int bookingTime;
        [ObservableProperty]
        private int cancelTime;
        [ObservableProperty]
        private int ticketClassCount;

        // Trạng thái chỉnh sửa từng trường
        [ObservableProperty]
        private bool isEditingMaxAirports;
        [ObservableProperty]
        private bool isEditingMinFlightTime;
        [ObservableProperty]
        private bool isEditingMaxStopover;
        [ObservableProperty]
        private bool isEditingMinStopTime;
        [ObservableProperty]
        private bool isEditingMaxStopTime;
        [ObservableProperty]
        private bool isEditingBookingTime;
        [ObservableProperty]
        private bool isEditingCancelTime;
        [ObservableProperty]
        private bool isEditingTicketClassCount;

        // Trường nhập liệu khi chỉnh sửa
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editMaxAirports;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editMinFlightTime;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editMaxStopover;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editMinStopTime;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editMaxStopTime;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editBookingTime;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editCancelTime;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editTicketClassCount;

        public RegulationManagementViewModel()
        {
            // Giả lập dữ liệu, thực tế nên load từ DB
            _ = LoadRegulationAsync();
        }
        private async Task LoadRegulationAsync()
        {
            try
            {
                await using var context = new AirTicketDbContext();

                var regulation = await context.Quydinhs
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync();

                if (regulation is not null)
                {
                    MaxAirports = regulation.SoSanBay ?? 10;
                    MinFlightTime = regulation.ThoiGianBayToiThieu ?? 30;
                    MaxStopover = regulation.SoSanBayTgtoiDa ?? 2;
                    MinStopTime = regulation.TgdungMin ?? 10;
                    MaxStopTime = regulation.TgdungMax ?? 20;
                    BookingTime = regulation.TgdatVeChamNhat ?? 1;
                    CancelTime = regulation.TghuyDatVe ?? 1;
                    TicketClassCount = regulation.SoHangVe ?? 2;
                }
                else
                {
                    // gán mặc định
                    MaxAirports = 10; MinFlightTime = 30; MaxStopover = 2;
                    MinStopTime = 10; MaxStopTime = 20; BookingTime = 1;
                    CancelTime = 1; TicketClassCount = 2;

                    // thêm bản ghi mặc định
                    regulation = new Quydinh
                    {
                        SoSanBay = MaxAirports,
                        ThoiGianBayToiThieu = MinFlightTime,
                        SoSanBayTgtoiDa = MaxStopover,
                        TgdungMin = MinStopTime,
                        TgdungMax = MaxStopTime,
                        TgdatVeChamNhat = BookingTime,
                        TghuyDatVe = CancelTime,
                        SoHangVe = TicketClassCount
                    };

                    context.Quydinhs.Add(regulation);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Bạn có thể hiển thị lỗi lên giao diện:
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu.\n" + ex.Message,
                                "Lỗi kết nối",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                // Hoặc log lỗi
                Debug.WriteLine("Lỗi khi load regulation: " + ex);
            }
        }


        private bool CanSave()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
        // --- MaxAirports ---
        [RelayCommand]
        private void _EditMaxAirports()
        {
            EditMaxAirports = MaxAirports;
            IsEditingMaxAirports = true;
        }
        [RelayCommand]
        private async Task SaveMaxAirportsAsync()
        {
            if (!CanSave()) return;
            if (MaxAirports == EditMaxAirports)
            {
                IsEditingMaxAirports = false;
                return;
            }

            try
            {
                await using var context = new AirTicketDbContext();
                int currentAirportCount = await context.Sanbays.CountAsync();
                if (currentAirportCount > EditMaxAirports)
                {
                    MessageBox.Show(
                        $"Hiện có {currentAirportCount} sân bay, lớn hơn giới hạn mới ({EditMaxAirports}).\n" +
                        "Vui lòng xóa bớt sân bay hoặc đặt giới hạn lớn hơn.",
                        "Giới hạn không hợp lệ",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { SoSanBay = EditMaxAirports };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.SoSanBay = EditMaxAirports;
                }

                await context.SaveChangesAsync(); 
                MaxAirports = EditMaxAirports;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
            finally
            {
                IsEditingMaxAirports = false;
            }
        }
        [RelayCommand]
        private void CancelMaxAirports()
        {
            IsEditingMaxAirports = false;
        }

        // --- MinFlightTime ---
        [RelayCommand]
        private void _EditMinFlightTime()
        {
            EditMinFlightTime = MinFlightTime;
            IsEditingMinFlightTime = true;
        }
        [RelayCommand]
        private async Task SaveMinFlightTime()
        {
            if (!CanSave()) return;

            if (MinFlightTime == EditMinFlightTime)
            {
                IsEditingMinFlightTime = false;
                return;
            }
            MinFlightTime = EditMinFlightTime;
            IsEditingMinFlightTime = false;

            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { ThoiGianBayToiThieu = MinFlightTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.ThoiGianBayToiThieu = MinFlightTime;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelMinFlightTime()
        {
            IsEditingMinFlightTime = false;
        }

        // --- MaxStopover ---
        [RelayCommand]
        private void _EditMaxStopover()
        {
            EditMaxStopover = MaxStopover;
            IsEditingMaxStopover = true;
        }
        [RelayCommand]
        private async Task SaveMaxStopover()
        {
            if (!CanSave()) return;

            if (MaxStopover == EditMaxStopover)
            {
                IsEditingMaxStopover = false;
                return;
            }
            MaxStopover = EditMaxStopover;
            IsEditingMaxStopover = false;
            
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { SoSanBayTgtoiDa = MaxStopover };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.SoSanBayTgtoiDa = MaxStopover;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
            
        }
        [RelayCommand]
        private void CancelMaxStopover()
        {
            IsEditingMaxStopover = false;
        }

        // --- MinStopTime ---
        [RelayCommand]
        private void _EditMinStopTime()
        {
            EditMinStopTime = MinStopTime;
            IsEditingMinStopTime = true;
        }
        [RelayCommand]
        private async Task SaveMinStopTime()
        {
            if (!CanSave()) return;
            if (MinStopTime == EditMinStopTime)
            {
                IsEditingMinStopTime = false;
                return;
            }
            MinStopTime = EditMinStopTime;
            IsEditingMinStopTime = false;           
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TgdungMin = MinStopTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TgdungMin = MinStopTime;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelMinStopTime()
        {
            IsEditingMinStopTime = false;
        }

        // --- MaxStopTime ---
        [RelayCommand]
        private void _EditMaxStopTime()
        {
            EditMaxStopTime = MaxStopTime;
            IsEditingMaxStopTime = true;
        }
        [RelayCommand]
        private async Task SaveMaxStopTime()
        {
            if (!CanSave()) return;

            if (MaxStopTime == EditMaxStopTime)
            {
                IsEditingMaxStopTime = false;
                return;
            }
            MaxStopTime = EditMaxStopTime;
            IsEditingMaxStopTime = false;
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TgdungMax = MaxStopTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TgdungMax = MaxStopTime;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }

        }
        [RelayCommand]
        private void CancelMaxStopTime()
        {
            IsEditingMaxStopTime = false;
        }

        // --- BookingTime ---
        [RelayCommand]
        private void _EditBookingTime()
        {
            EditBookingTime = BookingTime;
            IsEditingBookingTime = true;
        }
        [RelayCommand]
        private async Task SaveBookingTime()
        {
            if (!CanSave()) return;

            if (BookingTime == EditBookingTime)
            {
                IsEditingBookingTime = false;
                return;
            }
            BookingTime = EditBookingTime;
            IsEditingBookingTime = false;            
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TgdatVeChamNhat = BookingTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TgdatVeChamNhat = BookingTime;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelBookingTime()
        {
            IsEditingBookingTime = false;
        }

        // --- CancelTime ---
        [RelayCommand]
        private void _EditCancelTime()
        {
            EditCancelTime = CancelTime;
            IsEditingCancelTime = true;
        }
        [RelayCommand]
        private async Task SaveCancelTime()
        {
            if (!CanSave()) return;

            if (CancelTime == EditCancelTime)
            {
                IsEditingCancelTime = false;
                return;
            }
            CancelTime = EditCancelTime;
            IsEditingCancelTime = false;
            
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TghuyDatVe = CancelTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TghuyDatVe = CancelTime;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelCancelTime()
        {
            IsEditingCancelTime = false;
        }
        [RelayCommand]
        private void _EditTicketClassCount()
        {
            EditTicketClassCount = TicketClassCount;
            IsEditingTicketClassCount = true;
        }
        [RelayCommand]
        private async Task SaveTicketClassCount()
        {
            if (!CanSave()) return;

            if (TicketClassCount == EditTicketClassCount)
            {
                IsEditingTicketClassCount = false;
                return;
            }
            TicketClassCount = EditTicketClassCount;
            IsEditingTicketClassCount = false;
            

            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { SoHangVe = TicketClassCount };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.SoHangVe = TicketClassCount;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được quy định.\n" + ex.Message,
                                "Lỗi CSDL",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelTicketClassCount()
        {
            IsEditingTicketClassCount = false;
        }
    }
}