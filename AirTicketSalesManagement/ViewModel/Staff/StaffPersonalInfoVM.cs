using System;
using System.ComponentModel;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public class StaffPersonalInfoVM : INotifyPropertyChanged
    {
        private string _maNv;
        private string _hoTen;
        private string _gioiTinh;
        private DateTime _ngaySinh;
        private string _soDienThoai;
        private string _email;
        private string _cccd;

        public string MaNv
        {
            get => _maNv;
            set { _maNv = value; OnPropertyChanged(nameof(MaNv)); }
        }

        public string HoTen
        {
            get => _hoTen;
            set { _hoTen = value; OnPropertyChanged(nameof(HoTen)); }
        }

        public string GioiTinh
        {
            get => _gioiTinh;
            set { _gioiTinh = value; OnPropertyChanged(nameof(GioiTinh)); }
        }

        public DateTime NgaySinh
        {
            get => _ngaySinh;
            set { _ngaySinh = value; OnPropertyChanged(nameof(NgaySinh)); }
        }

        public string SoDienThoai
        {
            get => _soDienThoai;
            set { _soDienThoai = value; OnPropertyChanged(nameof(SoDienThoai)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string Cccd
        {
            get => _cccd;
            set { _cccd = value; OnPropertyChanged(nameof(Cccd)); }
        }

        public StaffPersonalInfoVM()
        {
            // Dữ liệu mẫu (có thể thay bằng dữ liệu đăng nhập thực tế)
            MaNv = "NV001";
            HoTen = "Nguyễn Văn A";
            GioiTinh = "Nam";
            NgaySinh = new DateTime(1995, 5, 20);
            SoDienThoai = "0912345678";
            Email = "a.nguyen@example.com";
            Cccd = "123456789012";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
