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

namespace AirTicketSalesManagement.View.Customer
{
    /// <summary>
    /// Interaction logic for CustomerProfileView.xaml
    /// </summary>
    public partial class CustomerProfileView : UserControl
    {
        public CustomerProfileView()
        {
            InitializeComponent();
        }

        private void TabTongQuan_Click(object sender, RoutedEventArgs e)
        {
            TabTongQuan.IsChecked = true;
            TabBaoMat.IsChecked = false;
            TongQuanContent.Visibility = Visibility.Visible;
            BaoMatContent.Visibility = Visibility.Collapsed;
        }

        private void TabBaoMat_Click(object sender, RoutedEventArgs e)
        {
            TabBaoMat.IsChecked = true;
            TabTongQuan.IsChecked = false;
            BaoMatContent.Visibility = Visibility.Visible;
            TongQuanContent.Visibility = Visibility.Collapsed;
        }

        private void TabBaoMat_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
