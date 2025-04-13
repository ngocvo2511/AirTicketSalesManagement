using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AirTicketSalesManagement.ViewModel
{
    public class MenuBarViewModel : BaseViewModel
    {
        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleExpandCommand { get; }

        //public MenuBarViewModel()
        //{
        //    ToggleExpandCommand = new RelayCommand(_ => IsExpanded = !IsExpanded);
        //}
    }

}
