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
	SoGioBay FLOAT,
	TTKhaiThac NVARCHAR(30)
)
select * from CHUYENBAY

CREATE TABLE SANBAYTRUNGGIAN (
	STT INT,
    MaSBTG CHAR(3) FOREIGN KEY REFERENCES SANBAY(MaSB),
    SoHieuCB VARCHAR(10) FOREIGN KEY REFERENCES CHUYENBAY(SoHieuCB),
    ThoiGianDung INT,
    GhiChu NVARCHAR(255),
	PRIMARY KEY (STT, SoHieuCB)
)
select * from SANBAYTRUNGGIAN

CREATE TABLE LICHBAY
(
	MaLB INT IDENTITY(1,1) PRIMARY KEY,
	SoHieuCB VARCHAR(10) FOREIGN KEY REFERENCES CHUYENBAY(SoHieuCB),
	GioDi DATETIME,
	GioDen DATETIME,
	LoaiMB NVARCHAR(30),
	SLVeKT INT,
	GiaVe MONEY,
	TTLichBay NVARCHAR(30)
)
select * from LICHBAY
select * from HANGVETHEOLICHBAY
select * from CHUYENBAY
select * from DATVE
select * from CTDV
select * from TAIKHOAN
CREATE TABLE HANGVE
(
	MaHV INT IDENTITY(1,1) PRIMARY KEY,
	TenHV NVARCHAR(30),
	HeSoGia FLOAT
)

CREATE TABLE HANGVETHEOLICHBAY (
    MaHV_LB INT IDENTITY PRIMARY KEY,
    MaLB INT FOREIGN KEY REFERENCES LICHBAY(MaLB),
    MaHV INT FOREIGN KEY REFERENCES HANGVE(MaHV),
    SLVeToiDa INT,
    SLVeConLai INT
)

CREATE TABLE KHACHHANG
(
	MaKH INT IDENTITY(1,1) PRIMARY KEY,
	HoTenKH NVARCHAR(30),
	GioiTinh NVARCHAR(10),
	NgaySinh DATE,
	SoDT CHAR(10),
	CCCD CHAR(12)
)

CREATE TABLE NHANVIEN (
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    HoTenNV NVARCHAR(50),
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    SoDT CHAR(10),
    CCCD CHAR(12)
)
select * from TAIKHOAN
delete from NHANVIEN where MaNV=2
CREATE TABLE DATVE
(
	MaDV INT IDENTITY(1,1) PRIMARY KEY,
	MaLB INT FOREIGN KEY REFERENCES LICHBAY(MaLB),
	MaKH INT FOREIGN KEY REFERENCES KHACHHANG(MaKH),
	MaNV INT FOREIGN KEY REFERENCES NHANVIEN(MaNV),
	ThoiGianDV DATETIME,
	SLVe INT,
	SoDTLienLac CHAR(10),
	Email VARCHAR(254),
	TongTienTT MONEY,
	TTDatVe NVARCHAR(30)
)
select * from DATVE
update CHUYENBAY set HangHangKhong = 'Vietjet Air' where SoGioBay=1.5

CREATE TABLE CTDV
(
	MaCTDV INT IDENTITY(1,1) PRIMARY KEY,
	MaDV INT FOREIGN KEY REFERENCES DATVE(MaDV),
	HoTenHK NVARCHAR(30),
	GioiTinh NVARCHAR(10),
	NgaySinh DATE,
	CCCD CHAR(12),
	HoTenNguoiGiamHo NVARCHAR(30),
	MaHV_LB INT FOREIGN KEY REFERENCES HANGVETHEOLICHBAY(MaHV_LB),
	GiaVeTT MONEY
)
DROP TABLE HangVeTheoLichBay

CREATE TABLE QUYDINH (
    ID INT IDENTITY(1,1) PRIMARY KEY,             
    SoSanBay INT,
    ThoiGianBayToiThieu INT,        
    SoSanBayTGToiDa INT,
    TGDungMin INT,                 
    TGDungMax INT,                 
    SoHangVe INT,
    TGDatVeChamNhat INT,
	TGHuyDatVe INT
)

CREATE TABLE TAIKHOAN(
	MaTK INT IDENTITY(1,1) PRIMARY KEY,
	Email VARCHAR(254) UNIQUE,
    MatKhau VARCHAR(100) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL CHECK (VaiTro IN (N'Admin', N'Nhân viên', N'Khách hàng')),
    MaNV INT FOREIGN KEY REFERENCES NHANVIEN(MaNV),
    MaKH INT FOREIGN KEY REFERENCES KHACHHANG(MaKH)
)

INSERT INTO NHANVIEN(HoTenNV) values ('Ngoc Vo Xuan')
select * from SANBAYTRUNGGIAN where SoHieuCB='VN109'
select * from TAIKHOAN
select * from NHANVIEN

INSERT INTO TAIKHOAN(Email, MatKhau, VaiTro, MaNV) values ('ad@ad.com', '$2a$12$X6ZEfqxwVBMDc0j9zzibyu9btxvTSNEuHDaADPgBnAEf0oRwn/QSO', 'Admin', 1)
update TAIKHOAN set VaiTro = 'NhanVien' where Email = 'nv@nv.com'
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


INSERT INTO CHUYENBAY (SoHieuCB, SBDi, SBDen, HangHangKhong, SoGioBay, TTKhaiThac)
VALUES 
('VN101', 'SGN', 'HAN', N'Vietnam Airlines', 2.0, N'Đang khai thác'),
('VN102', 'HAN', 'DAD', N'Vietnam Airlines', 1.2, N'Đang khai thác'),
('VJ205', 'SGN', 'DAD', N'Vietjet Air', 1.5, N'Đang khai thác'),
('QH301', 'HAN', 'CXR', N'Bamboo Airways', 1.7, N'Đang khai thác');

INSERT INTO SANBAYTRUNGGIAN (STT, MaSBTG, SoHieuCB, ThoiGianDung, GhiChu)
VALUES 
(1, 'DAD', 'VN101', 30, N'Dừng kỹ thuật'),
(1, 'VCA', 'VN102', 25, N'Tiếp nhiên liệu');

INSERT INTO LICHBAY (SoHieuCB, GioDi, GioDen, LoaiMB, SLVeKT, GiaVe, TTLichBay)
VALUES 
('VN101', '2025-04-25 08:00', '2025-04-25 10:00', N'Airbus A321', 200, 1500000, N'Chờ cất cánh'),
('VN102', '2025-04-25 13:00', '2025-04-25 14:15', N'Boeing 787', 150, 1200000, N'Chờ cất cánh'),
('VJ205', '2025-04-26 09:00', '2025-04-26 10:30', N'Airbus A320', 180, 1000000, N'Chờ cất cánh'),
('QH301', '2025-04-26 16:00', '2025-04-26 17:45', N'Embraer E190', 120, 1300000, N'Chờ cất cánh');

INSERT INTO HANGVE (TenHV, HeSoGia)
VALUES
(N'Phổ thông', 1),
(N'Thương gia', 1.05)
select * from HANGVETHEOLICHBAY
INSERT INTO HANGVETHEOLICHBAY(MaLB, MaHV, SLVeToiDa, SLVeConLai)
VALUES
(1, 1, 100, 100),
(1, 2, 100, 100),
(2, 1, 50, 50),
(2, 1, 100, 100),
(3, 1, 180, 180),
(4, 2, 120, 120)



INSERT INTO KHACHHANG (HoTenKH, GioiTinh, NgaySinh, Email, SoDT, CCCD)
VALUES 
(N'Nguyễn Văn A', N'Nam', '1990-05-20', 'vana@example.com', '0912345678', '0123456789'),
(N'Trần Thị B', N'Nữ', '1995-08-15', 'thib@example.com', '0987654321', '9876543210'),
(N'Lê Văn C', N'Nam', '1988-12-30', 'vanc@example.com', '0909123456', '4567891230'),
(N'Phạm Thị D', N'Nữ', '2000-02-10', 'thid@example.com', '0934567890', '3216549870');
 

select * from DATVE inner join CTDV on DATVE.MADV = CTDV.MADV

select * from TAIKHOAN
select * from KHACHHANG
select * from DATVE
delete from DATVE
delete from CTDV

select * from CTDV
set dateformat dmy

select * from TAIKHOAN
select * from NHANVIEN