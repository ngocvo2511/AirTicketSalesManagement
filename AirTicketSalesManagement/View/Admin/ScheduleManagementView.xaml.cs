using AirTicketSalesManagement.Models;
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

namespace AirTicketSalesManagement.View.Admin
{
    /// <summary>
    /// Interaction logic for ScheduleManagementView.xaml
    /// </summary>
    public partial class ScheduleManagementView : UserControl
    {

        public ScheduleManagementView()
        {
            InitializeComponent();
        }


        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void EditButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // CHẶN hoàn toàn sự kiện lan ra DataGridRow
        }

        private object _lastSelectedItem = null;

        private void dgLichBay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgLichBay.SelectedItem == _lastSelectedItem)
            {
                dgLichBay.SelectedItem = null; // Bỏ chọn → ẩn chi tiết
                _lastSelectedItem = null;
            }
            else
            {
                _lastSelectedItem = dgLichBay.SelectedItem;
            }
        }

    }

}
