using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.ViewModel.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.View.Customer;

namespace AirTicketSalesManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


        }

    }
}

    
