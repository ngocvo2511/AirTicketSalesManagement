﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Model
{
    public class ChuyenBay
    {
        private string _maChuyenBay;
        public string MaChuyenBay
        {
            get { return _maChuyenBay; }
            set { _maChuyenBay = value; }
        }
        private string _sanBayDi;
        public string SanBayDi
        {
            get { return _sanBayDi; }
            set { _sanBayDi = value; }
        }
        private string _sanBayDen;
        public string SanBayDen
        {
            get { return _sanBayDen; }
            set { _sanBayDen = value; }
        }
        private float _soGioBay;
        public float SoGioBay
        {
            get { return _soGioBay; }
            set { _soGioBay = value; }
        }
        private string _tinhTrangKhaiThac;
        public string TinhTrangKhaiThac
        {
            get { return _tinhTrangKhaiThac; }
            set { _tinhTrangKhaiThac = value; }
        }
    }
}
