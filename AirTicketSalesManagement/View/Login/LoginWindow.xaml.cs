using System;
using System.Windows;
using System.Windows.Controls;

namespace AirTicketSalesManagement.View.Login
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Sự kiện khi người dùng click vào Đăng nhập
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Kiểm tra tính hợp lệ của username và password
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!");
                return;
            }

            // Giả sử đăng nhập thành công
            MessageBox.Show($"Đăng nhập thành công cho {username}!");
        }

        // Sự kiện khi người dùng click vào Đăng ký
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string email = EmailTextBox.Text;
            string phone = PhoneNumberTextBox.Text;

            // Kiểm tra các trường nhập liệu
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                return;
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp không
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp!");
                return;
            }

            // Giả sử đăng ký thành công
            MessageBox.Show("Đăng ký thành công!");
        }

        // Sự kiện khi người dùng click vào Quên mật khẩu
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            // Chuyển đến Tab "Quên mật khẩu"
            TabControl.SelectedIndex = 2;  // Đảm bảo TabControl có tên "TabControl"
        }

        // Sự kiện khi người dùng gửi yêu cầu quên mật khẩu
        private void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            string emailOrPhone = PhoneNumberTextBox.Text; // Email hoặc Số điện thoại

            // Kiểm tra xem người dùng có nhập email hoặc số điện thoại không
            if (string.IsNullOrEmpty(emailOrPhone))
            {
                MessageBox.Show("Vui lòng nhập email hoặc số điện thoại!");
                return;
            }

            // Giả sử gửi yêu cầu quên mật khẩu thành công
            MessageBox.Show($"Yêu cầu quên mật khẩu đã được gửi tới {emailOrPhone}!");
        }

        // Sự kiện khi người dùng chọn mã vùng
        private void CountryCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCountry = CountryCodeComboBox.SelectedItem as ComboBoxItem;

            // Kiểm tra xem SelectedItem có null không
            if (selectedCountry != null)
            {
                string countryCode = selectedCountry.Content.ToString();
                // Chỉ lấy phần mã vùng từ text và điền vào PhoneNumberTextBox
                string code = countryCode.Substring(countryCode.IndexOf('(') + 1, countryCode.IndexOf(')') - countryCode.IndexOf('(') - 1);
                PhoneNumberTextBox.Text = code;
            }
            else
            {
                // Xử lý trường hợp không có sự chọn nào trong ComboBox
                MessageBox.Show("Vui lòng chọn mã vùng!");
            }
        }
    }
}
