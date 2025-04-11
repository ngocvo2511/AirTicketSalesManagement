using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AirTicketSalesManagement.ViewModel;

namespace AirTicketSalesManagement.Resources.User_Controls
{
    /// <summary>
    /// Interaction logic for CustomerMenuBar.xaml
    /// </summary>
    public partial class CustomerMenuBar : UserControl
    {
        public CustomerMenuBar()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.MenuBarViewModel();
        }
    }
}
