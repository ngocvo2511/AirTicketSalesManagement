using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AirTicketSalesManagement.Resources.User_Controls
{
    public partial class CustomTitleBar : UserControl
    {
        public CustomTitleBar()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (e.ClickCount == 2)
            {
                ToggleMaximize(window);
            }
            else
            {
                window?.DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            ToggleMaximize(window);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void ToggleMaximize(Window window)
        {
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }
    }
}
