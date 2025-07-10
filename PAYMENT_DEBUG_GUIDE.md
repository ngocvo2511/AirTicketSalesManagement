# Hướng dẫn Debug và Test Chức năng Thanh toán VNPay

## 🔍 Vấn đề đã được sửa

### Vấn đề chính:
1. **UserSession không được reset đúng cách khi logout**
2. **HandlePaymentSuccess() phụ thuộc vào UserSession cũ**
3. **Race condition khi chuyển tài khoản**

### Giải pháp đã áp dụng:
1. ✅ **Reset đầy đủ UserSession khi logout**
2. ✅ **Thêm logging chi tiết để debug**
3. ✅ **Cải thiện xử lý callback từ VNPay**
4. ✅ **Tạo PaymentDebugHelper để kiểm tra**

## 🧪 Cách Test và Debug

### 1. Test Scenario: Đăng xuất và chuyển tài khoản

```bash
# Bước 1: Đăng nhập tài khoản A
# Bước 2: Thực hiện thanh toán VNPay
# Bước 3: Đăng xuất
# Bước 4: Đăng nhập tài khoản B
# Bước 5: Thực hiện thanh toán VNPay
```

### 2. Kiểm tra Debug Logs

Mở **Output Window** trong Visual Studio và chọn **Debug** để xem logs:

```
=== USER SESSION DEBUG ===
AccountId: 1
CustomerId: 1
StaffId: null
CustomerName: Nguyễn Văn A
Email: user@example.com
isStaff: False
==========================

=== VALIDATING USER SESSION FOR PAYMENT ===
OK: Customer user with CustomerId: 1
=============================================

=== RECENT BOOKINGS DEBUG ===
Searching for customer bookings with MaKh: 1
Found 2 recent bookings:
  - MaDv: 123, MaKh: 1, MaNv: null, Status: Chưa thanh toán (Online), Time: 2024-01-15 10:30:00
  - MaDv: 124, MaKh: 1, MaNv: null, Status: Đã thanh toán, Time: 2024-01-15 10:25:00
=============================
```

### 3. Các điểm cần kiểm tra

#### ✅ **UserSession Reset**
- Khi logout, tất cả properties của UserSession phải được set về null/false
- Khi đăng nhập mới, UserSession phải được cập nhật đúng

#### ✅ **Booking Status**
- Booking phải được lưu với MaKh/MaNv đúng
- Status phải là "Chưa thanh toán (Online)" trước khi thanh toán
- Status phải chuyển thành "Đã thanh toán" sau khi thanh toán thành công

#### ✅ **VNPay Callback**
- Callback URL phải được xử lý đúng
- Payment result phải được parse chính xác
- HandlePaymentSuccess() phải được gọi với UserSession đúng

## 🐛 Các lỗi thường gặp và cách khắc phục

### Lỗi 1: "Thanh toán thành công nhưng vé không được cập nhật"

**Nguyên nhân:** UserSession không đúng khi HandlePaymentSuccess() được gọi

**Cách khắc phục:**
1. Kiểm tra debug logs để xem UserSession
2. Đảm bảo logout đã reset đầy đủ UserSession
3. Đảm bảo đăng nhập mới đã cập nhật đúng UserSession

### Lỗi 2: "Không tìm thấy vé để cập nhật"

**Nguyên nhân:** Booking được tìm với UserSession sai

**Cách khắc phục:**
1. Kiểm tra MaKh/MaNv trong database
2. Kiểm tra thời gian booking (phải trong 20 phút gần nhất)
3. Kiểm tra UserSession khi tìm booking

### Lỗi 3: "VNPay callback không được xử lý"

**Nguyên nhân:** WebView không bắt được callback URL

**Cách khắc phục:**
1. Kiểm tra callback URL trong VnpayPayment.cs
2. Kiểm tra WebView.NavigationStarting event
3. Kiểm tra debug logs của callback

## 📋 Checklist Test

### Test Case 1: Thanh toán bình thường
- [ ] Đăng nhập tài khoản
- [ ] Đặt vé và chọn VNPay
- [ ] Thanh toán thành công
- [ ] Kiểm tra vé được cập nhật thành "Đã thanh toán"

### Test Case 2: Đăng xuất và đăng nhập tài khoản khác
- [ ] Đăng nhập tài khoản A
- [ ] Đặt vé và thanh toán
- [ ] Đăng xuất
- [ ] Đăng nhập tài khoản B
- [ ] Đặt vé và thanh toán
- [ ] Kiểm tra cả 2 vé đều được xử lý đúng

### Test Case 3: Thanh toán thất bại
- [ ] Đặt vé và chọn VNPay
- [ ] Hủy thanh toán hoặc thanh toán thất bại
- [ ] Kiểm tra vé vẫn ở trạng thái "Chưa thanh toán (Online)"

## 🔧 Công cụ Debug

### PaymentDebugHelper
```csharp
// Log thông tin UserSession
PaymentDebugHelper.LogUserSession();

// Log các booking gần đây
PaymentDebugHelper.LogRecentBookings(customerId: 1);

// Log chi tiết một booking
PaymentDebugHelper.LogPaymentStatus(bookingId: 123);

// Validate UserSession
PaymentDebugHelper.ValidateUserSessionForPayment();
```

### Debug Output
Tất cả debug logs sẽ xuất hiện trong **Output Window** của Visual Studio với prefix:
- `[ProcessVNPayPayment]`
- `[HandlePaymentSuccess]`
- `[WebView_NavigationStarting]`
- `[SaveBookingWithPendingStatus]`

## 📞 Liên hệ hỗ trợ

Nếu gặp vấn đề, hãy:
1. Chụp màn hình debug logs
2. Mô tả các bước thực hiện
3. Gửi thông tin lỗi chi tiết 