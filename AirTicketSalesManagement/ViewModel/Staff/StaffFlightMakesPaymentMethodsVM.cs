using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using AirTicketSalesManagement.Helper;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    /// <summary>
    /// ViewModel xử lý logic thanh toán cho StaffFlightMakesPaymentMethods.xaml
    /// </summary>
    public class StaffFlightMakesPaymentMethodsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Danh sách các phương thức thanh toán có sẵn
        public ObservableCollection<string> AvailablePaymentMethods { get; } =
            new ObservableCollection<string>
            {
                "Tiền mặt",
                "Thẻ tín dụng",
                "Chuyển khoản"
            };

        // Thuộc tính binding
        private string _selectedPaymentMethod;
        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set { _selectedPaymentMethod = value; RaisePropertyChanged(nameof(SelectedPaymentMethod)); }
        }

        private string _cardNumber;
        public string CardNumber
        {
            get => _cardNumber;
            set { _cardNumber = value; RaisePropertyChanged(nameof(CardNumber)); }
        }

        private string _cardHolderName;
        public string CardHolderName
        {
            get => _cardHolderName;
            set { _cardHolderName = value; RaisePropertyChanged(nameof(CardHolderName)); }
        }

        private string _expiryDate;
        public string ExpiryDate
        {
            get => _expiryDate;
            set { _expiryDate = value; RaisePropertyChanged(nameof(ExpiryDate)); }
        }

        private string _cvv;
        public string CVV
        {
            get => _cvv;
            set { _cvv = value; RaisePropertyChanged(nameof(CVV)); }
        }

        // Command xác nhận thanh toán
        public ICommand ConfirmPaymentCommand { get; }

        public StaffFlightMakesPaymentMethodsVM()
        {
            // Khởi tạo command với logic mẫu
            ConfirmPaymentCommand = new RelayCommand(_ => ExecutePayment());
        }

        private void ExecutePayment()
        {
            // TODO: Thực hiện logic thanh toán (gọi service, lưu DB,...)
            // Ví dụ:
            // PaymentService.Process(SelectedPaymentMethod, CardNumber, CardHolderName, ExpiryDate, CVV);
        }
    }
}