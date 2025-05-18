using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public class ScheduleItem : INotifyPropertyChanged
    {
        public string FlightCode { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class StaffManageSchedulesVM : INotifyPropertyChanged
    {
        private string _flightCode;
        private DateTime _scheduleDate = DateTime.Now;
        private string _status;

        public string FlightCode
        {
            get => _flightCode;
            set { _flightCode = value; OnPropertyChanged(nameof(FlightCode)); }
        }

        public DateTime ScheduleDate
        {
            get => _scheduleDate;
            set { _scheduleDate = value; OnPropertyChanged(nameof(ScheduleDate)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public ObservableCollection<ScheduleItem> Schedules { get; set; }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public StaffManageSchedulesVM()
        {
            Schedules = new ObservableCollection<ScheduleItem>
            {
                new ScheduleItem { FlightCode = "VN001", Date = DateTime.Today, Status = "Đã cất cánh" },
                new ScheduleItem { FlightCode = "VN002", Date = DateTime.Today.AddDays(1), Status = "Chờ khởi hành" }
            };

            AddCommand = new RelayCommand(AddSchedule);
            EditCommand = new RelayCommand(EditSchedule);
            DeleteCommand = new RelayCommand(DeleteSchedule);
        }

        private void AddSchedule(object obj)
        {
            Schedules.Add(new ScheduleItem
            {
                FlightCode = this.FlightCode,
                Date = this.ScheduleDate,
                Status = this.Status
            });

            FlightCode = string.Empty;
            Status = string.Empty;
        }

        private void EditSchedule(object obj)
        {
            if (obj is ScheduleItem s)
            {
                s.FlightCode = this.FlightCode;
                s.Date = this.ScheduleDate;
                s.Status = this.Status;
            }
        }

        private void DeleteSchedule(object obj)
        {
            if (obj is ScheduleItem s && Schedules.Contains(s))
            {
                Schedules.Remove(s);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
