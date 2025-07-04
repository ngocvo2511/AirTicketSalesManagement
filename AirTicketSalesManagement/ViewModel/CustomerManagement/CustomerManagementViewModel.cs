using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.CustomerManagement
{
    public partial class CustomerManagementViewModel : BaseViewModel
    {
        private ObservableCollection<Khachhang> _customers = new();
        [ObservableProperty]
        private string searchName;
        [ObservableProperty]
        private string searchCccd;
        [ObservableProperty]
        private ObservableCollection<Khachhang> customers = new ObservableCollection<Khachhang>();
        [ObservableProperty]
        private Khachhang selectedCustomer;

        //Edit
        [ObservableProperty]
        private bool isEditPopupOpen = false;
        [ObservableProperty]
        private string? editName;
        [ObservableProperty]
        private string? editCccd;
        [ObservableProperty]
        private string? editPhone;
        [ObservableProperty]
        private string? editGender;
        [ObservableProperty]
        private DateTime? editBirthDate;

        public CustomerManagementViewModel()
        {
            _ = LoadCustomers();
        }
        public async Task LoadCustomers()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var result = await context.Khachhangs.ToListAsync();
                    _customers = new ObservableCollection<Khachhang>(result);
                    Customers = new ObservableCollection<Khachhang>(result);
                }
            }
            catch (Exception e)
            {
                // Handle exception (optional logging or user notification)
            }
        }
        [RelayCommand]
        public void EditCustomer()
        {
            if (SelectedCustomer == null) return;
            LoadEditField();
            IsEditPopupOpen = true;
        }
        private void LoadEditField()
        {
            if (SelectedCustomer == null) return;
            EditName = SelectedCustomer.HoTenKh;
            EditGender = SelectedCustomer.GioiTinh;
            EditCccd = SelectedCustomer.Cccd;
            EditPhone = SelectedCustomer.SoDt;
            EditBirthDate = SelectedCustomer.NgaySinh?.ToDateTime(new TimeOnly(0, 0));
        }
        [RelayCommand]
        public void Refresh()
        {
            _ = LoadCustomers();
        }
        [RelayCommand]
        public void Search()
        {
            var query = _customers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchName))
            {
                query = query.Where(c =>
                    c.HoTenKh?.Contains(SearchName, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (!string.IsNullOrWhiteSpace(SearchCccd))
            {
                query = query.Where(c => c.Cccd?.Contains(SearchCccd) == true);
            }
            Customers.Clear();
            foreach (var c in query)
                Customers.Add(c);
        }
        [RelayCommand]
        public void ClearSearch()
        {
            SearchName = string.Empty;
            SearchCccd = string.Empty;
            Customers = new ObservableCollection<Khachhang>(_customers);
        }

        [RelayCommand]
        public void CancelEdit()
        {
            IsEditPopupOpen = false;
        }
        [RelayCommand]
        public async Task SaveEditCustomer()
        {
            if (string.IsNullOrWhiteSpace(EditName))
            {
                MessageBox.Show("Tên không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(EditPhone) && !IsValidPhone(EditPhone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(EditCccd) && (EditCccd.Length != 12 || !EditCccd.All(char.IsDigit)))
            {
                MessageBox.Show("Số căn cước không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (EditBirthDate.HasValue)
            {
                if (EditBirthDate.Value.Date >= DateTime.Today)
                {
                    MessageBox.Show("Ngày sinh không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            try
            {
                await using var context = new AirTicketDbContext();
                var customer = await context.Khachhangs.FindAsync(SelectedCustomer.MaKh);

                if (customer is null)
                {
                    MessageBox.Show("Không tìm thấy khách hàng trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Cập nhật dữ liệu
                customer.HoTenKh = EditName;
                customer.GioiTinh = EditGender;
                customer.Cccd = EditCccd;
                customer.SoDt = EditPhone;
                customer.NgaySinh = EditBirthDate.HasValue ? DateOnly.FromDateTime(EditBirthDate.Value) : null;

                await context.SaveChangesAsync();

                MessageBox.Show("Cập nhật khách hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadCustomers();

                IsEditPopupOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{8,10}$");
        }     
    }
}
    
    
