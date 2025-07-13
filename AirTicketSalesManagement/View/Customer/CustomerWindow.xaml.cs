using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Customer;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using VNPAY.NET.Models;
using VNPAY.NET;
using Microsoft.Web.WebView2.Core;
using AirTicketSalesManagement.ViewModel;
using Microsoft.Web.WebView2.Wpf;
using AirTicketSalesManagement.Messages;

namespace AirTicketSalesManagement.View.Customer
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private readonly NotificationViewModel notification = new();

        public NotificationViewModel NotificationViewModel
        {
            get => notification;
        }

        public CustomerWindow()
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

            // Lắng nghe message để clear cache và ẩn WebView
            WeakReferenceMessenger.Default.Register<WebViewClearCacheMessage>(this, async (r, m) =>
            {
                if (WebView != null && WebView.CoreWebView2 != null)
                {
                    // Xóa toàn bộ cookies (mọi domain)
                    await WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCookies", "{}");

                    // Xóa toàn bộ dữ liệu website (local storage, cache, indexedDB, ... trên tất cả origin)
                    await WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Storage.clearDataForOrigin",
                        "{\"origin\":\"*\",\"storageTypes\":\"all\"}");
                }
            });
        }

        private async void WebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("https://localhost:1234/api/Vnpay/Callback"))
            {
                var query = new Uri(e.Uri).Query;
                e.Cancel = true;

                System.Diagnostics.Debug.WriteLine($"[WebView_NavigationStarting] VNPay callback received: {e.Uri}");
                System.Diagnostics.Debug.WriteLine($"[WebView_NavigationStarting] Query string: {query}");

                await Dispatcher.InvokeAsync(async () =>
                {
                    var viewModel = DataContext as CustomerViewModel;

                    try
                    {
                        var result = HandlePaymentResult(query);
                        System.Diagnostics.Debug.WriteLine($"[WebView_NavigationStarting] Payment result: {result.IsSuccess}, Message: {result.ToString()}");

                        if (result.IsSuccess)
                        {
                            if (WebView != null && WebView.CoreWebView2 != null)
                            {
                                // Xóa toàn bộ cookies (mọi domain)
                                await WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCookies", "{}");

                                // Xóa toàn bộ dữ liệu website (local storage, cache, indexedDB, ... trên tất cả origin)
                                await WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Storage.clearDataForOrigin",
                                    "{\"origin\":\"*\",\"storageTypes\":\"all\"}");
                            }
                            if (viewModel != null) viewModel.IsWebViewVisible = false;
                            await notification.ShowNotificationAsync(
                                "Thanh toán thành công!",
                                NotificationType.Information);
                            WeakReferenceMessenger.Default.Send(new PaymentSuccessMessage());
                        }
                        else
                        {
                            if (WebView != null && WebView.CoreWebView2 != null)
                            {
                                // Xóa toàn bộ cookies (mọi domain)
                                await WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCookies", "{}");

                                // Xóa toàn bộ dữ liệu website (local storage, cache, indexedDB, ... trên tất cả origin)
                                await WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Storage.clearDataForOrigin",
                                    "{\"origin\":\"*\",\"storageTypes\":\"all\"}");
                            }
                            if (viewModel != null) viewModel.IsWebViewVisible = false;
                            await notification.ShowNotificationAsync(
                                "Thanh toán thất bại: " + result.ToString(),
                                NotificationType.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[WebView_NavigationStarting] Exception: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[WebView_NavigationStarting] Stack trace: {ex.StackTrace}");

                        if (viewModel != null) viewModel.IsWebViewVisible = false;
                        await notification.ShowNotificationAsync(
                            "Đã xảy ra lỗi: " + ex.Message,
                            NotificationType.Error);
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