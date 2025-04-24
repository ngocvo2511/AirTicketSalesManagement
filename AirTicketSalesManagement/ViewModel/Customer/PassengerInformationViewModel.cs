using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public class PassengerInformationViewModel : BaseViewModel
    {
        #region Properties

        // Flight details
        public string FlightCode { get; set; }

        // Passenger list
        private ObservableCollection<PassengerInfoModel> _passengerList;
        public ObservableCollection<PassengerInfoModel> PassengerList
        {
            get { return _passengerList; }
            set
            {
                _passengerList = value;
                OnPropertyChanged();
            }
        }

        // Contact information
        private string _contactEmail;
        public string ContactEmail
        {
            get { return _contactEmail; }
            set
            {
                _contactEmail = value;
                OnPropertyChanged();
            }
        }

        private string _contactPhone;
        public string ContactPhone
        {
            get { return _contactPhone; }
            set
            {
                _contactPhone = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand BackCommand { get; private set; }
        public ICommand ContinueCommand { get; private set; }

        #endregion

        public PassengerInformationViewModel()
        {
            // Initialize commands
            BackCommand = new RelayCommand(ExecuteBackCommand);
            ContinueCommand = new RelayCommand(ExecuteContinueCommand, CanExecuteContinueCommand);

            // Initialize with test data (should be replaced with actual data from previous screen)
            FlightCode = "VN123 (SGN-HAN)";
            InitializePassengerList(2, 1, 1); // Example: 2 adults, 1 child, 1 infant
        }

        private void InitializePassengerList(int adultCount, int childCount, int infantCount)
        {
            PassengerList = new ObservableCollection<PassengerInfoModel>();

            // Add adults
            for (int i = 0; i < adultCount; i++)
            {
                PassengerList.Add(new PassengerInfoModel
                {
                    PassengerType = PassengerType.Adult,
                    Index = i + 1,
                    GenderOptions = new List<string> { "Nam", "Nữ" }
                });
            }

            // Add children
            for (int i = 0; i < childCount; i++)
            {
                PassengerList.Add(new PassengerInfoModel
                {
                    PassengerType = PassengerType.Child,
                    Index = i + 1,
                    GenderOptions = new List<string> { "Nam", "Nữ" }
                });
            }

            // Add infants
            for (int i = 0; i < infantCount; i++)
            {
                var infant = new PassengerInfoModel
                {
                    PassengerType = PassengerType.Infant,
                    Index = i + 1,
                    GenderOptions = new List<string> { "Nam", "Nữ" }
                };

                // Set list of adults for accompanying dropdown
                infant.AdultPassengers = PassengerList.Where(p => p.PassengerType == PassengerType.Adult).ToList();

                PassengerList.Add(infant);
            }
        }

        private void ExecuteBackCommand(object parameter)
        {
            // Navigate back to flight search/selection page
            // Implementation depends on your navigation framework
        }

        private bool CanExecuteContinueCommand(object parameter)
        {
            // Validate all required fields are filled
            if (string.IsNullOrWhiteSpace(ContactEmail) || string.IsNullOrWhiteSpace(ContactPhone))
                return false;

            foreach (var passenger in PassengerList)
            {
                if (string.IsNullOrWhiteSpace(passenger.FullName) || passenger.Gender == null ||
                    passenger.DateOfBirth == null)
                    return false;

                // Additional validation for adults
                if (passenger.PassengerType == PassengerType.Adult &&
                    string.IsNullOrWhiteSpace(passenger.IdentityNumber))
                    return false;

                // Additional validation for infants
                if (passenger.PassengerType == PassengerType.Infant &&
                    passenger.AccompanyingAdult == null)
                    return false;
            }

            return true;
        }

        private void ExecuteContinueCommand(object parameter)
        {
            // Process passenger information and continue to next step (payment or confirmation)
            // Implementation depends on your application flow
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class PassengerInfoModel : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public PassengerType PassengerType { get; set; }

        public bool IsAdult => PassengerType == PassengerType.Adult;
        public bool IsChild => PassengerType == PassengerType.Child;
        public bool IsInfant => PassengerType == PassengerType.Infant;

        public string PassengerTypeText
        {
            get
            {
                return PassengerType switch
                {
                    PassengerType.Adult => "Người lớn",
                    PassengerType.Child => "Trẻ em",
                    PassengerType.Infant => "Em bé",
                    _ => string.Empty
                };
            }
        }

        public string PassengerTypeDescription
        {
            get
            {
                return PassengerType switch
                {
                    PassengerType.Adult => "Từ 12 tuổi trở lên",
                    PassengerType.Child => "Từ 2-12 tuổi",
                    PassengerType.Infant => "Dưới 2 tuổi",
                    _ => string.Empty
                };
            }
        }

        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public List<string> GenderOptions { get; set; }

        private DateTime? _dateOfBirth;
        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged();
            }
        }

        private string _identityNumber;
        public string IdentityNumber
        {
            get { return _identityNumber; }
            set
            {
                _identityNumber = value;
                OnPropertyChanged();
            }
        }

        // For infants only
        public List<PassengerInfoModel> AdultPassengers { get; set; }

        private PassengerInfoModel _accompanyingAdult;
        public PassengerInfoModel AccompanyingAdult
        {
            get { return _accompanyingAdult; }
            set
            {
                _accompanyingAdult = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public enum PassengerType
    {
        Adult,
        Child,
        Infant
    }

    // Simple relay command implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
