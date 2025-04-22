using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using AirTicketSalesManagement.Helper;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public class StaffFlightSelectingTicketsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        public ObservableCollection<Seat> AvailableSeats { get; }
        public ICommand ConfirmSelectionCommand { get; }

        public StaffFlightSelectingTicketsVM()
        {
            AvailableSeats = new ObservableCollection<Seat>();
            for (int i = 1; i <= 30; i++)
                AvailableSeats.Add(new Seat { SeatNumber = i.ToString() });
            ConfirmSelectionCommand = new RelayCommand(_ => ConfirmSelection());
        }

        void ConfirmSelection()
        {
            var selected = new ObservableCollection<Seat>();
            foreach (var s in AvailableSeats)
                if (s.IsSelected)
                    selected.Add(s);
            // xử lý selected
        }
    }

    public class Seat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        private string _seatNumber;
        public string SeatNumber
        {
            get => _seatNumber;
            set { _seatNumber = value; OnPropertyChanged(nameof(SeatNumber)); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }
    }
}
