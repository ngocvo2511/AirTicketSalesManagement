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
using System.Globalization;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;

namespace AirTicketSalesManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                System.Windows.Markup.XmlLanguage.GetLanguage("en-GB"))); // British culture for dd/MM/yyyy
            base.OnStartup(e);
            QuestPDF.Settings.License = LicenseType.Community;
        }

    }
}

    
