using System;
using System.Windows;

namespace AirTicketSalesManagement.View.Register
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        // Sự kiện khi người dùng chọn mã quốc gia
        private void CountryCodeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Lấy mã vùng đã chọn
            var selectedCountry = (System.Windows.Controls.ComboBoxItem)CountryCodeComboBox.SelectedItem;
            string countryCode = selectedCountry.Content.ToString();
            // Bạn có thể xử lý mã vùng (countryCode) ở đây nếu cần
        }

        // Sự kiện khi người dùng nhấn nút "Đăng ký"
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Lấy các giá trị nhập vào từ người dùng
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string email = EmailTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text;

            // Kiểm tra mật khẩu có khớp không
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp!");
                return;
            }

            // Kiểm tra các trường thông tin có hợp lệ không
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                return;
            }

            // Thực hiện logic đăng ký ở đây (ví dụ: lưu vào cơ sở dữ liệu)
            // Code thực tế đăng ký tài khoản sẽ được thực hiện ở đây

            MessageBox.Show("Đăng ký thành công!");
            this.Close();  // Đóng cửa sổ đăng ký sau khi thành công
        }
    }
}
