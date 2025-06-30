using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;

namespace AirTicketSalesManagement.ViewModel
{
    public enum NotificationType
    {
        Information,
        Warning,
        Error
    }

    public partial class NotificationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private bool isVisible;

        [ObservableProperty]
        private Brush background;

        public NotificationViewModel(){}

        [RelayCommand]
        private void Close()
        {
            IsVisible = false;
        }

        public void ShowNotification(string message, NotificationType type)
        {
            Message = message;
            IsVisible = true;

            switch (type)
            {
                case NotificationType.Information:
                    Title = "Thông báo";
                    Background = Brushes.DodgerBlue;
                    break;
                case NotificationType.Warning:
                    Title = "Cảnh báo";
                    Background = Brushes.Orange;
                    break;
                case NotificationType.Error:
                    Title = "Lỗi";
                    Background = Brushes.Crimson;
                    break;
            }
        }
    }
}