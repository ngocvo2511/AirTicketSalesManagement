using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public class StaffFlightScheduleLookUpVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public string FromAirport { get; set; }
        public string ToAirport { get; set; }
        public DateTime? DepartureDate { get; set; }

        public ObservableCollection<FlightSchedule> FlightSchedules { get; }
            = new ObservableCollection<FlightSchedule>();

        public ICommand SearchCommand { get; }

        public StaffFlightScheduleLookUpVM()
        {
            DepartureDate = DateTime.Today;
            SearchCommand = new RelayCommand(_ => LoadSchedules());
        }

        private void LoadSchedules()
        {
            FlightSchedules.Clear();
            // TODO: gọi service/db để load dữ liệu thực
            FlightSchedules.Add(new FlightSchedule
            {
                FlightCode = "VN001",
                DepartureTime = "08:00",
                ArrivalTime = "10:30",
                SeatsAvailable = 5
            });
        }
    }

    public class FlightSchedule
    {
        public string FlightCode { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public int SeatsAvailable { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public RelayCommand(Action<object> execute) => _execute = execute;
        public bool CanExecute(object _) => true;
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged;
    }
}
