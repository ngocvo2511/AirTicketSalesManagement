using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Interface;
using AirTicketSalesManagement.Services.EmailServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;

namespace AirTicketSalesManagement.ViewModel.Login
{
    public partial class ResetPasswordViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly EmailTemplateService _emailTemplateService;
        private AuthViewModel _auth;
        private readonly Dictionary<string, List<string>> _errors = new();
        private ToastViewModel _toast = new ToastViewModel();
        [ObservableProperty]
        private string? code;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private bool isCodeValid;

        [ObservableProperty]
        private bool isCodeExpired;

        [ObservableProperty]
        private string? timeLeftText;

        [ObservableProperty]
        private bool canResendCode;
        public string? Email { get; set; }

        private DispatcherTimer? _timer;
        private TimeSpan _timeLeft;

        public ResetPasswordViewModel(AuthViewModel _auth, string Email, IEmailService emailService, IOtpService otpService, EmailTemplateService emailTemplateService)
        {
            _emailService = emailService;
            _otpService = otpService;
            _emailTemplateService = emailTemplateService;
            StartCountdown();
            this._auth = _auth;
            this.Email = Email;
            string otp = _otpService.GenerateOtp(Email);
            var emailContent = _emailTemplateService.BuildForgotPasswordOtp(otp);
            _emailService.SendEmailAsync(Email, "Yêu cầu đặt lại mật khẩu", emailContent).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _ = _toast.ShowToastAsync("Không thể gửi mã xác nhận. Vui lòng thử lại sau.", Brushes.OrangeRed);
                }
                else
                {
                    _ = _toast.ShowToastAsync("Mã xác nhận đã được gửi đến email của bạn.", Brushes.Green);
                }
            });
        }
        [RelayCommand]        
        
        private async Task CheckCode()
        {
            ClearErrors(nameof(Code));
            if (IsCodeExpired)
            {
                AddError(nameof(Code), "Mã xác nhận đã hết hạn. Vui lòng gửi lại mã mới.");
                return;
            }

            if (Email == null)
            {
                AddError(nameof(Email), "Email không được để trống.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Code))
            {
                AddError(nameof(Code), "Mã xác nhận không được để trống.");
                return;
            }

            bool isValid = _otpService.VerifyOtp(Email,Code);
            if (isValid)
            {
                IsCodeValid = true;
            }
            else
            {
                AddError(nameof(Code), "Mã xác nhận không hợp lệ.");
                IsCodeValid = false;
            }
        }

        [RelayCommand]
        private async Task ResendCodeAsync()
        {
            string otp = _otpService.GenerateOtp(Email);
            var emailContent = _emailTemplateService.BuildForgotPasswordOtp(otp);
            await _emailService.SendEmailAsync(Email, "Yêu cầu đặt lại mật khẩu", emailContent);
            ResetCountdown();
        }

        private void StartCountdown()
        {
            _timeLeft = TimeSpan.FromMinutes(3);
            IsCodeExpired = false;
            CanResendCode = false;
            UpdateTimeLeftText();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) =>
            {
                _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));
                UpdateTimeLeftText();

                if (_timeLeft <= TimeSpan.Zero)
                {
                    _timer?.Stop();
                    IsCodeExpired = true;
                    CanResendCode = true;
                    TimeLeftText = "Mã xác nhận đã hết hạn.";
                }
            };
            _timer.Start();
        }

        private void ResetCountdown()
        {
            _timer?.Stop();
            StartCountdown();
        }

        private void UpdateTimeLeftText()
        {
            TimeLeftText = $"Mã hết hạn sau: {_timeLeft.Minutes:D2}:{_timeLeft.Seconds:D2}";
        }

        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            if (!IsCodeValid)
            {
                AddError(nameof(Code), "Vui lòng xác nhận mã trước khi đặt lại mật khẩu.");
                return;
            }
            Validate();
            if (HasErrors) return;
            try
            {
                using(var context = new AirTicketDbContext())
                {
                    var user = context.Taikhoans.FirstOrDefault(x => x.Email == Email);
                    if (user == null)
                    {
                        AddError(nameof(Email), "Tài khoản không tồn tại");
                        return;
                    }
                    string hashPass = BCrypt.Net.BCrypt.HashPassword(Password);
                    user.MatKhau = hashPass;
                    await context.SaveChangesAsync();
                }
                _auth.CurrentViewModel = new LoginViewModel(_auth);
            }
            catch
            {
                await _toast.ShowToastAsync("Không thể kết nối đến cơ sở dữ liệu", Brushes.OrangeRed);
                return;
            }
            
        }
        #region Error
        public void Validate()
        {
            ClearErrors(nameof(Password));
            ClearErrors(nameof(ConfirmPassword));

            if (string.IsNullOrWhiteSpace(Password))
                AddError(nameof(Password), "Mật khẩu không được để trống.");
            else if (Password.Length > 100)
                AddError(nameof(Password), "Mật khẩu vượt quá giới hạn cho phép");
            if (ConfirmPassword != Password)
            {
                AddError(nameof(ConfirmPassword), "Xác nhận mật khẩu không khớp với mật khẩu.");
            }
            if (HasErrors) return;
        }
        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable? GetErrors(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName) && _errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return null;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        #endregion

        [RelayCommand]
        private void ShowLogin() => _auth.NavigateToLogin();
    }

}
