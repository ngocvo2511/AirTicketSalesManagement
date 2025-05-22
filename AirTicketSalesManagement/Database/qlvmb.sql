CREATE DATABASE QLVMB
USE QLVMB
Drop database QLVMB
CREATE TABLE SANBAY
(
	MaSB CHAR(3) PRIMARY KEY,
	TenSB NVARCHAR(50),
	ThanhPho NVARCHAR(30), 
	QuocGia NVARCHAR(30)
)


CREATE TABLE CHUYENBAY
(
	SoHieuCB VARCHAR(10) PRIMARY KEY,
	SBDi CHAR(3) FOREIGN KEY REFERENCES SANBAY(MaSB),
	SBDen CHAR(3) FOREIGN KEY REFERENCES SANBAY(MaSB),
	HangHangKhong NVARCHAR(50),
	MayBay VARCHAR(30),
	SoGioBay FLOAT,
	TTKhaiThac NVARCHAR(30)
)

CREATE TABLE SANBAYTRUNGGIAN (
	STT INT,
    MaSBTG CHAR(3) FOREIGN KEY REFERENCES SANBAY(MaSB),
    SoHieuCB VARCHAR(10) FOREIGN KEY REFERENCES CHUYENBAY(SoHieuCB),
    ThoiGianDung INT,
    GhiChu NVARCHAR(255),
	PRIMARY KEY (STT, SoHieuCB)
)

CREATE TABLE LICHBAY
(
	MaLB VARCHAR(10) PRIMARY KEY,
	SoHieuCB VARCHAR(10) FOREIGN KEY REFERENCES CHUYENBAY(SoHieuCB),
	GioDi DATETIME,
	GioDen DATETIME,
	LoaiMB NVARCHAR(30),
	SLVeKT INT,
	GiaVe MONEY,
	TTLichBay NVARCHAR(30)
)

CREATE TABLE LOAIVE
(
	MaLV VARCHAR(10) PRIMARY KEY,
	MaLB VARCHAR(10) FOREIGN KEY REFERENCES LICHBAY(MaLB),
	HangGhe NVARCHAR(30),
	HeSoGia FLOAT,
	SLVeToiDa INT,
	SLVeConLai INT
)

CREATE TABLE KHACHHANG
(
	MaKH VARCHAR(10) PRIMARY KEY,
	HoTenKH NVARCHAR(30),
	GioiTinh NVARCHAR(10),
	NgaySinh DATE,
	Email VARCHAR(254) UNIQUE,
	SoDT CHAR(10),
	GiayToTT VARCHAR(30)
)

CREATE TABLE DATVE
(
	MaDV VARCHAR(10) PRIMARY KEY,
	MaLB VARCHAR(10) FOREIGN KEY REFERENCES LICHBAY(MaLB),
	MaKH VARCHAR(10) FOREIGN KEY REFERENCES KHACHHANG(MaKH),
	ThoiGianDV DATETIME,
	SLVe INT,
	SoDTLienLac CHAR(10),
	Email VARCHAR(254),
	TongTienTT MONEY,
	TTDatVe NVARCHAR(30)
)

CREATE TABLE CTDV
(
	MaCTDV VARCHAR(10) PRIMARY KEY,
	MaDV VARCHAR(10) FOREIGN KEY REFERENCES DATVE(MaDV),
	HoTenHK NVARCHAR(30),
	GioiTinh NVARCHAR(10),
	NgaySinh DATE,
	GiayToTuyThan VARCHAR(12),
	HoTenNguoiGiamHo NVARCHAR(30),
	MaLV VARCHAR(10) FOREIGN KEY REFERENCES LOAIVE(MaLV),
	GiaVeTT MONEY
)

CREATE TABLE QUYDINH (
    ID INT PRIMARY KEY,             
    SoSanBay INT,
    ThoiGianBayToiThieu INT,        
    SoSanBayTGToiDa INT,
    TGDungMin INT,                 
    TGDungMax INT,                 
    SoHangVe INT,
    TGDatVeChamNhat INT,
	TGHuyDatVe INT
)
CREATE TABLE NHANVIEN (
    MaNV VARCHAR(10) PRIMARY KEY,
    HoTenNV NVARCHAR(50),
    GioiTinh NVARCHAR(10)
    NgaySinh DATE,
    Email VARCHAR(254) UNIQUE,
    SoDT CHAR(10),
    CCCD VARCHAR(12) UNIQUE
)
CREATE TABLE TAIKHOAN(
	Email VARCHAR(254) PRIMARY KEY,
    MatKhau VARCHAR(100) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL CHECK (VaiTro IN ('Admin', 'QuanLy', 'NhanVien', 'KhachHang')),
    MaNV VARCHAR(10) FOREIGN KEY REFERENCES NHANVIEN(MaNV),
    MaKH VARCHAR(10) FOREIGN KEY REFERENCES KHACHHANG(MaKH)
)

DELETE FROM SANBAY
-- Sân bay quốc tế
INSERT INTO SANBAY (MaSB, TenSB, ThanhPho, QuocGia) VALUES
('SGN', N'Sân bay quốc tế Tân Sơn Nhất', N'TP. Hồ Chí Minh', N'Việt Nam'),
('HAN', N'Sân bay quốc tế Nội Bài', N'Hà Nội', N'Việt Nam'),
('DAD', N'Sân bay quốc tế Đà Nẵng', N'Đà Nẵng', N'Việt Nam'),
('CXR', N'Sân bay quốc tế Cam Ranh', N'Khánh Hòa', N'Việt Nam'),
('HPH', N'Sân bay quốc tế Cát Bi', N'Hải Phòng', N'Việt Nam'),
('HUI', N'Sân bay quốc tế Phú Bài', N'Thừa Thiên - Huế', N'Việt Nam'),
('VCA', N'Sân bay quốc tế Cần Thơ', N'Cần Thơ', N'Việt Nam'),
('PQC', N'Sân bay quốc tế Phú Quốc', N'Kiên Giang', N'Việt Nam'),
('VII', N'Sân bay quốc tế Vinh', N'Nghệ An', N'Việt Nam'),
('DLI', N'Sân bay quốc tế Liên Khương', N'Lâm Đồng', N'Việt Nam'),
('VDO', N'Sân bay quốc tế Vân Đồn', N'Quảng Ninh', N'Việt Nam');

-- Sân bay nội địa
INSERT INTO SANBAY (MaSB, TenSB, ThanhPho, QuocGia) VALUES
('BMV', N'Sân bay Buôn Ma Thuột', N'Đắk Lắk', N'Việt Nam'),
('CAH', N'Sân bay Cà Mau', N'Cà Mau', N'Việt Nam'),
('VCL', N'Sân bay Chu Lai', N'Quảng Nam', N'Việt Nam'),
('VCS', N'Sân bay Côn Đảo', N'Bà Rịa - Vũng Tàu', N'Việt Nam'),
('DIN', N'Sân bay Điện Biên Phủ', N'Điện Biên', N'Việt Nam'),
('VDH', N'Sân bay Đồng Hới', N'Quảng Bình', N'Việt Nam'),
('PXU', N'Sân bay Pleiku', N'Gia Lai', N'Việt Nam'),
('UIH', N'Sân bay Phù Cát', N'Bình Định', N'Việt Nam'),
('VKG', N'Sân bay Rạch Giá', N'Kiên Giang', N'Việt Nam'),
('THD', N'Sân bay Thọ Xuân', N'Thanh Hóa', N'Việt Nam'),
('TBB', N'Sân bay Tuy Hòa', N'Phú Yên', N'Việt Nam')


INSERT INTO CHUYENBAY (SoHieuCB, SBDi, SBDen, HangHangKhong, MayBay, SoGioBay, TTKhaiThac)
VALUES 
('VN101', 'SGN', 'HAN', N'Vietnam Airlines', 'Airbus A321', 2.0, N'Đang khai thác'),
('VN102', 'HAN', 'DAD', N'Vietnam Airlines', 'Boeing 787', 1.2, N'Đang khai thác'),
('VJ205', 'SGN', 'DAD', N'Vietjet Air', 'Airbus A320', 1.5, N'Đang khai thác'),
('QH301', 'HAN', 'CXR', N'Bamboo Airways', 'Embraer E190', 1.7, N'Đang khai thác');

INSERT INTO SANBAYTRUNGGIAN (STT, MaSBTG, SoHieuCB, ThoiGianDung, GhiChu)
VALUES 
(1, 'DAD', 'VN101', 30, N'Dừng kỹ thuật'),
(1, 'VCA', 'VN102', 25, N'Tiếp nhiên liệu');

INSERT INTO LICHBAY (MaLB, SoHieuCB, GioDi, GioDen, LoaiMB, SLVeKT, GiaVe, TTLichBay)
VALUES 
('LB001', 'VN101', '2025-04-25 08:00', '2025-04-25 10:00', N'Airbus A321', 200, 1500000, N'Còn chỗ'),
('LB002', 'VN102', '2025-04-25 13:00', '2025-04-25 14:15', N'Boeing 787', 150, 1200000, N'Còn chỗ'),
('LB003', 'VJ205', '2025-04-26 09:00', '2025-04-26 10:30', N'Airbus A320', 180, 1000000, N'Còn chỗ'),
('LB004', 'QH301', '2025-04-26 16:00', '2025-04-26 17:45', N'Embraer E190', 120, 1300000, N'Còn chỗ');

INSERT INTO LOAIVE (MaLV, MaLB, HangGhe, HeSoGia, SLVeToiDa, SLVeConLai)
VALUES 
('LV001', 'LB001', N'Phổ thông', 1.0, 150, 100),
('LV002', 'LB001', N'Thương gia', 1.5, 50, 30),
('LV003', 'LB002', N'Phổ thông', 1.0, 100, 70),
('LV004', 'LB002', N'Thương gia', 1.6, 50, 20),
('LV005', 'LB003', N'Phổ thông', 1.0, 180, 90),
('LV006', 'LB004', N'Phổ thông', 1.0, 100, 60),
('LV007', 'LB004', N'Hạng đặc biệt', 2.0, 20, 10);


INSERT INTO KHACHHANG (MaKH, HoTenKH, GioiTinh, NgaySinh, Email, SoDT, GiayToTT)
VALUES 
('KH001', N'Nguyễn Văn A', N'Nam', '1990-05-20', 'vana@example.com', '0912345678', '0123456789'),
('KH002', N'Trần Thị B', N'Nữ', '1995-08-15', 'thib@example.com', '0987654321', '9876543210'),
('KH003', N'Lê Văn C', N'Nam', '1988-12-30', 'vanc@example.com', '0909123456', '4567891230'),
('KH004', N'Phạm Thị D', N'Nữ', '2000-02-10', 'thid@example.com', '0934567890', '3216549870');


select * from DATVE inner join CTDV on DATVE.MADV = CTDV.MADV

select * from TAIKHOAN
select * from DATVE
select * from CTDV

INSERT INTO DATVE (MaDV, MaLB, MaKH, ThoiGianDV, SLVe, SoDTLienLac, Email, TongTienTT, TTDatVe)
VALUES
('DV001', 'LB001', 'KH00000005', '2025-05-20 14:30:00', 2, '0912345678', 'khach1@example.com', 3000000, N'Đã thanh toán'),
('DV002', 'LB002', 'KH00000005', '2025-05-21 09:15:00', 1, '0987654321', 'khach2@example.com', 1500000, N'Chưa thanh toán')


INSERT INTO CTDV (MaCTDV, MaDV, HoTenHK, GioiTinh, NgaySinh, GiayToTuyThan, HoTenNguoiGiamHo, MaLV, GiaVeTT)
VALUES
('CT001', 'DV001', N'Nguyễn Văn A', N'Nam', '1990-01-01', '012345678901', NULL, 'LV001', 1500000),
('CT002', 'DV001', N'Lê Thị B', N'Nữ', '1992-02-02', '098765432109', NULL, 'LV001', 1500000),
('CT003', 'DV002', N'Trần Văn C', N'Nam', '1985-03-03', '123123123123', NULL, 'LV002', 1500000);
