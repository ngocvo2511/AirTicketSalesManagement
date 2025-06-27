using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Customer;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Core;
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
using System.Windows.Shapes;
using VNPAY.NET.Models;
using VNPAY.NET;
using AirTicketSalesManagement.ViewModel.Staff;

namespace AirTicketSalesManagement.View.Staff
{
    /// <summary>
    /// Interaction logic for StaffWindow.xaml
    /// </summary>
    public partial class StaffWindow : Window
    {
        public StaffWindow()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<WebViewNavigationMessage>(this, async (r, m) =>
            {
                await WebView.EnsureCoreWebView2Async();
                // Xóa các event handler cũ nếu có
                WebView.NavigationStarting -= WebView_NavigationStarting;

                // Gắn lại handler
                WebView.NavigationStarting += WebView_NavigationStarting;

                WebView.Source = new Uri(m.Value);

            });
        }

        private async void WebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("https://localhost:1234/api/Vnpay/Callback"))
            {
                var query = new Uri(e.Uri).Query;
                e.Cancel = true;

                await Dispatcher.InvokeAsync(() =>
                {
                    var viewModel = DataContext as StaffViewModel;

                    try
                    {
                        var result = HandlePaymentResult(query);
                        if (result.IsSuccess)
                        {
                            if (viewModel != null) viewModel.IsWebViewVisible = false;
                            MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            WeakReferenceMessenger.Default.Send(new PaymentSuccessMessage());
                        }
                        else
                        {
                            if (viewModel != null) viewModel.IsWebViewVisible = false;
                            MessageBox.Show("Thanh toán thất bại: " + result.ToString(), "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (viewModel != null) viewModel.IsWebViewVisible = false;
                        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        private PaymentResult HandlePaymentResult(string query)
        {
            // Parse query string thành NameValueCollection
            var parsed = System.Web.HttpUtility.ParseQueryString(query);

            // Tạo Dictionary an toàn, không mất key null hoặc trùng
            var dict = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();

            foreach (var key in parsed.AllKeys)
            {
                if (!string.IsNullOrEmpty(key))  // Bỏ qua key null
                {
                    dict[key] = parsed[key]; // Microsoft.Extensions.Primitives.StringValues tự nhận chuỗi
                }
            }

            var queryCollection = new Microsoft.AspNetCore.Http.Internal.QueryCollection(dict);

            // Tạo và khởi tạo VNPAY
            var vnpay = new Vnpay();
            vnpay.Initialize(
                tmnCode: "B2G07RMI",
                hashSecret: "GWB7ENE65XM4QCX0SD1GC8UWTUH0PJS2",
                baseUrl: "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
                callbackUrl: "https://localhost:1234/api/Vnpay/Callback"
            );
            System.Diagnostics.Debug.WriteLine("==== QUERY COLLECTION ====");
            foreach (var kv in queryCollection)
            {
                System.Diagnostics.Debug.WriteLine($"{kv.Key} = {kv.Value}");
            }
            // Xử lý kết quả
            return vnpay.GetPaymentResult(queryCollection);
        }
    }
}
