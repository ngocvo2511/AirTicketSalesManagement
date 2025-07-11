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
        [ObservableProperty]
        private int infantAge;
        [ObservableProperty]
        private int childAge;

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
        [ObservableProperty]
        private bool isEditingInfantAge;
        [ObservableProperty]
        private bool isEditingChildAge;

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
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editInfantAge;
        [ObservableProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị phải ≥ 0")]
        [NotifyDataErrorInfo]
        private int editChildAge;

        // Notification
        public NotificationViewModel Notification { get; set; } = new NotificationViewModel();

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
                    CancelTime = regulation.TghuyDatVe ?? 0;
                    TicketClassCount = regulation.SoHangVe ?? 2;
                    InfantAge = regulation.TuoiToiDaSoSinh ?? 2;
                    ChildAge = regulation.TuoiToiDaTreEm ?? 12;
                }
                else
                {
                    // gán mặc định
                    MaxAirports = 10; MinFlightTime = 30; MaxStopover = 2;
                    MinStopTime = 10; MaxStopTime = 20; BookingTime = 1;
                    CancelTime = 1; TicketClassCount = 2; InfantAge = 2; ChildAge = 12;

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
                        SoHangVe = TicketClassCount,
                        TuoiToiDaSoSinh = InfantAge,
                        TuoiToiDaTreEm = ChildAge
                    };

                    context.Quydinhs.Add(regulation);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi lên giao diện
                await Notification.ShowNotificationAsync("Không thể kết nối đến cơ sở dữ liệu.\n" + ex.Message, NotificationType.Error);

                // Log lỗi
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
        public async void SaveMaxAirports()
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
                    await Notification.ShowNotificationAsync(
                        $"Hiện có {currentAirportCount} sân bay, lớn hơn giới hạn mới ({EditMaxAirports}).\n" +
                        "Vui lòng xóa bớt sân bay hoặc đặt giới hạn lớn hơn.",
                        NotificationType.Warning);
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
                IsEditingMaxAirports = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
                Debug.WriteLine(ex);
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
        private async void SaveMinFlightTime()
        {
            if (!CanSave()) return;

            if (MinFlightTime == EditMinFlightTime)
            {
                IsEditingMinFlightTime = false;
                return;
            }

            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { ThoiGianBayToiThieu = EditMinFlightTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.ThoiGianBayToiThieu = EditMinFlightTime;
                }

                await context.SaveChangesAsync();
                MinFlightTime = EditMinFlightTime;
                IsEditingMinFlightTime = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
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
        private async void SaveMaxStopover()
        {
            if (!CanSave()) return;

            if (MaxStopover == EditMaxStopover)
            {
                IsEditingMaxStopover = false;
                return;
            }

            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { SoSanBayTgtoiDa = EditMaxStopover };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.SoSanBayTgtoiDa = EditMaxStopover;
                }

                await context.SaveChangesAsync();
                MaxStopover = EditMaxStopover;
                IsEditingMaxStopover = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
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
        private async void SaveMinStopTime()
        {
            if (!CanSave()) return;
            if (MinStopTime == EditMinStopTime)
            {
                IsEditingMinStopTime = false;
                return;
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TgdungMin = EditMinStopTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TgdungMin = EditMinStopTime;
                }

                await context.SaveChangesAsync();
                MinStopTime = EditMinStopTime;
                IsEditingMinStopTime = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
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
        private async void SaveMaxStopTime()
        {
            if (!CanSave()) return;

            if (MaxStopTime == EditMaxStopTime)
            {
                IsEditingMaxStopTime = false;
                return;
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TgdungMax = EditMaxStopTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TgdungMax = EditMaxStopTime;
                }

                await context.SaveChangesAsync();
                MaxStopTime = EditMaxStopTime;
                IsEditingMaxStopTime = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
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
        private async void SaveBookingTime()
        {
            if (!CanSave()) return;

            if (BookingTime == EditBookingTime)
            {
                IsEditingBookingTime = false;
                return;
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TgdatVeChamNhat = EditBookingTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TgdatVeChamNhat = EditBookingTime;
                }

                await context.SaveChangesAsync();
                BookingTime = EditBookingTime;
                IsEditingBookingTime = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
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
        private async void SaveCancelTime()
        {
            if (!CanSave()) return;

            if (CancelTime == EditCancelTime)
            {
                IsEditingCancelTime = false;
                return;
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TghuyDatVe = EditCancelTime };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.TghuyDatVe = EditCancelTime;
                }

                await context.SaveChangesAsync();
                CancelTime = EditCancelTime;
                IsEditingCancelTime = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
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
        private async void SaveTicketClassCount()
        {
            if (!CanSave()) return;

            if (TicketClassCount == EditTicketClassCount)
            {
                IsEditingTicketClassCount = false;
                return;
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { SoHangVe = EditTicketClassCount };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    regulation.SoHangVe = EditTicketClassCount;
                }

                await context.SaveChangesAsync();
                TicketClassCount = EditTicketClassCount;
                IsEditingTicketClassCount = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelTicketClassCount()
        {
            IsEditingTicketClassCount = false;
        }

        [RelayCommand]
        private void _EditInfantAge()
        {
            EditInfantAge = InfantAge;
            IsEditingInfantAge = true;
        }
        [RelayCommand]
        private async void SaveInfantAge()
        {
            if (!CanSave()) return;
            if (InfantAge == EditInfantAge)
            {
                IsEditingInfantAge = false;
                return;
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();

                if (regulation is null)
                {
                    regulation = new Quydinh { TuoiToiDaSoSinh = EditInfantAge };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    int currentChildAge = regulation.TuoiToiDaTreEm ?? 12;
                    if (EditInfantAge >= currentChildAge)
                    {
                        await Notification.ShowNotificationAsync(
                            "Tuổi tối đa của trẻ sơ sinh phải nhỏ hơn tuổi tối đa của trẻ em.\n" +
                            $"Hiện tại, tuổi tối đa của trẻ em là {currentChildAge}.",
                            NotificationType.Warning);
                        return;
                    }
                    regulation.TuoiToiDaSoSinh = EditInfantAge;
                }

                await context.SaveChangesAsync();
                InfantAge = EditInfantAge;
                IsEditingInfantAge = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelInfantAge()
        {
            IsEditingInfantAge = false;
        }

        [RelayCommand]
        private void _EditChildAge()
        {
            EditChildAge = ChildAge;
            IsEditingChildAge = true;
        }
        [RelayCommand]
        private async void SaveChildAge()
        {
            if (!CanSave()) return;
            if (ChildAge == EditChildAge)
            {
                IsEditingChildAge = false;
                return;
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var regulation = await context.Quydinhs.FirstOrDefaultAsync();
                if (regulation is null)
                {
                    regulation = new Quydinh { TuoiToiDaTreEm = EditChildAge };
                    context.Quydinhs.Add(regulation);
                }
                else
                {
                    int currentInfantAge = regulation.TuoiToiDaSoSinh ?? 2;
                    if (EditChildAge <= currentInfantAge)
                    {
                        await Notification.ShowNotificationAsync(
                            "Tuổi tối đa của trẻ em phải lớn hơn tuổi tối đa của trẻ sơ sinh.\n" +
                            $"Hiện tại, tuổi tối đa của trẻ sơ sinh là {currentInfantAge}.",
                            NotificationType.Warning);
                        return;
                    }
                    regulation.TuoiToiDaTreEm = EditChildAge;
                }
                await context.SaveChangesAsync();
                ChildAge = EditChildAge;
                IsEditingChildAge = false;
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Không lưu được quy định.\n" + ex.Message, NotificationType.Error);
                Debug.WriteLine(ex);
            }
        }
        [RelayCommand]
        private void CancelChildAge()
        {
            IsEditingChildAge = false;
        }
    }
}