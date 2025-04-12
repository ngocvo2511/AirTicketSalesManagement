using System.Windows;

namespace AirTicketSalesManagement.View.Login
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Logic đăng nhập
            MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.ShowDialog();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register.RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}