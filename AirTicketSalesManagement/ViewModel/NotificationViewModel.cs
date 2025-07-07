using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;

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
        private Brush borderBrush;

        [ObservableProperty]
        private Brush foreground;

        [ObservableProperty]
        private DropShadowEffect shadowEffect;

        [ObservableProperty]
        private bool showYesNo;

        private static readonly DropShadowEffect InfoEffect = new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 20,
            ShadowDepth = 5,
            Opacity = 0.4
        };

        private static readonly DropShadowEffect WarningEffect = new DropShadowEffect
        {
            Color = Colors.DarkOrange,
            BlurRadius = 25,
            ShadowDepth = 4,
            Opacity = 0.5
        };

        private static readonly DropShadowEffect ErrorEffect = new DropShadowEffect
        {
            Color = Colors.Red,
            BlurRadius = 30,
            ShadowDepth = 6,
            Opacity = 0.6
        };

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
                    BorderBrush = Brushes.White;
                    Foreground = Brushes.White;
                    //ShadowEffect = InfoEffect;
                    break;

                case NotificationType.Warning:
                    Title = "Cảnh báo";
                    Background = Brushes.Orange;
                    BorderBrush = Brushes.White;
                    Foreground = Brushes.White;
                    //ShadowEffect = WarningEffect;
                    break;

                case NotificationType.Error:
                    Title = "Lỗi";
                    Background = Brushes.Crimson;
                    BorderBrush = Brushes.White;
                    Foreground = Brushes.White;
                    //ShadowEffect = ErrorEffect;
                    break;
            }

            return _tcs.Task;
        }
    }
}