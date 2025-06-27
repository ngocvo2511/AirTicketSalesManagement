using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models
{
    public partial class SBTG : ObservableObject
    {
        [ObservableProperty]
        private int sTT;
        [ObservableProperty]
        private string maSBTG;
        [ObservableProperty]
        private int thoiGianDung;
        [ObservableProperty]
        private string ghiChu;

        [ObservableProperty]
        private ObservableCollection<string> sbtgList;

        public Action? OnMaSBTGChangedCallback { get; set; }

        partial void OnMaSBTGChanged(string value)
        {
            OnMaSBTGChangedCallback?.Invoke();
        }
    }
}
