using System.Windows;

namespace AirTicketSalesManagement.View.Login
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            // Logic gửi yêu cầu quên mật khẩu
            MessageBox.Show("Yêu cầu đặt lại mật khẩu đã được gửi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}