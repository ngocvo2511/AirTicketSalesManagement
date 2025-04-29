using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
namespace AirTicketSalesManagement.ViewModel.Staff
{
    public class Flight : INotifyPropertyChanged
    {
        public string FlightCode { get; set; }
        public string FromAirport { get; set; }
        public string ToAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class StaffManageFlightsVM : INotifyPropertyChanged
    {
        private string _flightCode;
        private string _fromAirport;
        private string _toAirport;

        public string FlightCode
        {
            get => _flightCode;
            set { _flightCode = value; OnPropertyChanged(nameof(FlightCode)); }
        }

        public string FromAirport
        {
            get => _fromAirport;
            set { _fromAirport = value; OnPropertyChanged(nameof(FromAirport)); }
        }

        public string ToAirport
        {
            get => _toAirport;
            set { _toAirport = value; OnPropertyChanged(nameof(ToAirport)); }
        }

        public ObservableCollection<Flight> Flights { get; set; }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public StaffManageFlightsVM()
        {
            Flights = new ObservableCollection<Flight>
            {
                new Flight { FlightCode = "VN001", FromAirport = "Hà Nội", ToAirport = "TP.HCM", DepartureTime = DateTime.Now.AddHours(2), ArrivalTime = DateTime.Now.AddHours(4) },
                new Flight { FlightCode = "VN002", FromAirport = "TP.HCM", ToAirport = "Đà Nẵng", DepartureTime = DateTime.Now.AddHours(5), ArrivalTime = DateTime.Now.AddHours(7) }
            };

            AddCommand = new RelayCommand(AddFlight);
            EditCommand = new RelayCommand(EditFlight);
            DeleteCommand = new RelayCommand(DeleteFlight);
        }

        private void AddFlight(object obj)
        {
            var newFlight = new Flight
            {
                FlightCode = this.FlightCode,
                FromAirport = this.FromAirport,
                ToAirport = this.ToAirport,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(3)
            };

            Flights.Add(newFlight);

            FlightCode = string.Empty;
            FromAirport = string.Empty;
            ToAirport = string.Empty;
        }

        private void EditFlight(object obj)
        {
            if (obj is Flight flight)
            {
                flight.FlightCode = this.FlightCode;
                flight.FromAirport = this.FromAirport;
                flight.ToAirport = this.ToAirport;
            }
        }

        private void DeleteFlight(object obj)
        {
            if (obj is Flight flight && Flights.Contains(flight))
            {
                Flights.Remove(flight);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
