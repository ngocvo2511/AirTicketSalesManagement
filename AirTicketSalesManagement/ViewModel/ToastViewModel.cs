using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AirTicketSalesManagement.ViewModel
{
    public partial class ToastViewModel : ObservableObject
    {
        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private bool isVisible;

        [ObservableProperty]
        private Brush background = Brushes.LightGreen;



        public async Task ShowToastAsync(string message, Brush bg = null, int durationMs = 2000)
        {
            if (IsVisible)
            {
                IsVisible = false;
                await Task.Delay(500);
            }
            Message = message;
            Background = bg ?? Brushes.LightGreen;
            IsVisible = true;

            await Task.Delay(durationMs);

            IsVisible = false;
        }
    }
}

