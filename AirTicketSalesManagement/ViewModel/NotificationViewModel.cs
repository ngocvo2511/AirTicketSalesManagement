using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
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

        [ObservableProperty]
        private bool showYesNo;

        private TaskCompletionSource<bool> _tcs;

        public NotificationViewModel(){}

        [RelayCommand]
        private void Yes()
        {
            Close(true);
        }

        [RelayCommand]
        private void No()
        {
            Close(false);
        }

        [RelayCommand]
        private void Close()
        {
            Close(false);
        }

        private void Close(bool result)
        {
            IsVisible = false;
            _tcs?.SetResult(result);
            _tcs = null;
        }

        public Task<bool> ShowNotificationAsync(string message, NotificationType type, bool isConfirmation = false)
        {
            Message = message;
            ShowYesNo = isConfirmation;
            IsVisible = true;
            _tcs = new TaskCompletionSource<bool>();

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

            return _tcs.Task;
        }
    }
}