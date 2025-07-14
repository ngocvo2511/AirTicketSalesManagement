using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace AirTicketSalesManagementTests.Helpers
{
    public static class ConfigurationHelper
    {
        private static IConfiguration _originalConfig;
        private static FieldInfo _configRootField;

        // Phương thức này sẽ tìm và lưu lại cấu hình gốc
        static ConfigurationHelper()
        {
            // Dùng Reflection để truy cập vào trường static private chứa configuration
            _configRootField = typeof(ConfigurationManager)
                .GetField("_configRoot", BindingFlags.NonPublic | BindingFlags.Static);

            if (_configRootField != null)
            {
                _originalConfig = (IConfiguration)_configRootField.GetValue(null);
            }
        }

        // Kích hoạt cấu hình Test
        public static void ActivateTestConfiguration()
        {
            if (_configRootField == null) return;

            var testConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false)
                .Build();

            _configRootField.SetValue(null, testConfig);
        }

        // Khôi phục cấu hình gốc
        public static void RestoreOriginalConfiguration()
        {
            if (_configRootField != null && _originalConfig != null)
            {
                _configRootField.SetValue(null, _originalConfig);
            }
        }
    }

    // Cần phải có lớp ConfigurationManager giả lập này vì .NET Framework không có sẵn
    // trong khi EF Core tools có thể cần đến.
    // LƯU Ý: Đoạn này hơi phức tạp, nó truy cập vào cơ chế bên trong của .NET
    // để có thể ghi đè cấu hình một cách linh hoạt.
    // Trong các ứng dụng ASP.NET Core, việc này dễ dàng hơn nhiều.
    internal static class ConfigurationManager
    {
        public static IConfiguration AppSettings { get; }
        static ConfigurationManager()
        {
            AppSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
    }
}