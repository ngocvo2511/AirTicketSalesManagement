using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Models.UIModels
{
    public partial class HangVeTheoLichBay : ObservableObject
    {
        [ObservableProperty]
        private int iD;
        [ObservableProperty]
        private int sTT;
        [ObservableProperty]
        private string tenHangVe;
        [ObservableProperty]
        private string sLVeToiDa;
        [ObservableProperty]
        private string sLVeConLai;
        [ObservableProperty]
        private ObservableCollection<string> hangVeList;
        public Action? OnTenHangVeChangedCallback { get; set; }

        partial void OnTenHangVeChanged(string value)
        {
            OnTenHangVeChangedCallback?.Invoke();
        }

    }
}
