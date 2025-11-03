# 🏥 KTPM - Hệ thống Quản lý Nhà Thuốc

> Pharmacy Management System - Windows Forms Application (.NET/C#)

## 📋 Giới thiệu

Hệ thống quản lý nhà thuốc toàn diện với các tính năng:
- ✅ Quản lý thuốc (Medicines)
- ✅ Quản lý nhân viên (Employees)
- ✅ Quản lý khách hàng (Customers)
- ✅ Quản lý nhà cung cấp (Suppliers)
- ✅ Quản lý hóa đơn (Invoices)
- ✅ Quản lý phiếu nhập (Purchase Orders)
- ✅ Quản lý hợp đồng (Contracts)
- ✅ Phản hồi khách hàng (Customer Feedback)
- ✅ Thùng rác (Soft Delete/Trash Management)
- ✅ Phân quyền theo vai trò (Role-based Access Control)

## 🏗️ Kiến trúc

Dự án sử dụng **3-tier Architecture**:

```
┌─────────────────────────────────────────────┐
│              UI Layer (Forms)               │
│  LoginForm, MainForm, ThuocPanel, etc.      │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Controller Layer                    │
│  ThuocController, HoaDonController, etc.    │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│              DAO Layer                      │
│  ThuocDAO, HoaDonDAO, etc.                  │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│            Database (MySQL)                 │
│  Tables: Thuoc, HoaDon, NhanVien, etc.      │
└─────────────────────────────────────────────┘
```

### Cấu trúc thư mục:

```
KTPM/
├── Entities/          # 15 entity classes
│   ├── Thuoc.cs
│   ├── HoaDon.cs
│   ├── NhanVien.cs
│   └── ...
├── dao/              # 11 DAO classes (Data Access)
│   ├── ThuocDAO.cs
│   ├── HoaDonDAO.cs
│   └── ...
├── controller/       # 9 Controller classes
│   ├── ThuocController.cs
│   ├── HoaDonController.cs
│   └── ...
├── UI/               # 36 UI forms và panels
│   ├── LoginForm.cs
│   ├── MainForm.cs
│   ├── ThuocPanel.cs
│   └── ...
├── connectDB/        # Database connection utilities
│   ├── DBConnection.cs
│   └── DBCloseHelper.cs
└── ultis/            # 5 utility classes
    ├── Validator.cs
    ├── MessageDialog.cs
    ├── DateHelper.cs
    └── ...
```

## 📊 Thống kê Code

- **Tổng số file C#**: 71 files
- **Tổng số dòng code**: ~15,522 dòng
- **Entities**: 15 classes
- **DAOs**: 11 classes
- **Controllers**: 9 classes
- **UI Components**: 36 forms/panels
- **Utilities**: 7 classes

## 🔍 Code Review

### 📄 Các tài liệu review:

1. **[CODE_REVIEW.md](CODE_REVIEW.md)** - Code review tổng quan
   - Điểm mạnh và điểm yếu của dự án
   - 45+ vấn đề được xác định
   - Roadmap cải thiện
   - Code quality metrics

2. **[TECHNICAL_ANALYSIS.md](TECHNICAL_ANALYSIS.md)** - Phân tích kỹ thuật chi tiết
   - Phân tích từng layer
   - Database design review
   - Security analysis (OWASP Top 10)
   - Performance analysis
   - Code examples cụ thể

3. **[QUICK_FIX_GUIDE.md](QUICK_FIX_GUIDE.md)** - Hướng dẫn sửa lỗi nhanh
   - Step-by-step guide cho 5 vấn đề critical
   - Code examples đầy đủ
   - Checklist

### 🎯 Điểm số tổng quan

| Tiêu chí | Điểm | Trạng thái |
|----------|------|------------|
| Architecture | 8/10 | ✅ Tốt |
| Code Organization | 8/10 | ✅ Tốt |
| Security | 4/10 | ⚠️ Cần cải thiện |
| Error Handling | 6/10 | ⚠️ Trung bình |
| Performance | 6/10 | ⚠️ Trung bình |
| Maintainability | 7/10 | ✅ Tốt |
| Testing | 0/10 | ❌ Không có |
| Documentation | 5/10 | ⚠️ Trung bình |

**Tổng điểm**: **6.1/10** - Cần cải thiện

## 🔴 Vấn đề Critical (Cần fix ngay)

1. **Database Provider Inconsistency**
   - `DBConnection.cs` dùng SQL Server
   - `ThuocDAO.cs` và các DAO khác dùng MySQL
   - ❌ Code không thể compile

2. **Hard-coded Credentials**
   ```csharp
   "User Id=sa;Password=123123;"  // ❌ Security vulnerability
   ```

3. **Demo Authentication**
   ```csharp
   bool ok = (username == "admin" && password == "admin");  // ❌ Hard-coded
   ```

4. **No Password Hashing**
   - Passwords lưu plaintext trong database
   - ❌ Critical security issue

5. **Java-style Entities**
   - Sử dụng private fields + getter/setter methods
   - ❌ Không tương thích với C# ecosystem

👉 **Xem hướng dẫn chi tiết**: [QUICK_FIX_GUIDE.md](QUICK_FIX_GUIDE.md)

## ✅ Điểm mạnh

1. **Kiến trúc 3-tier rõ ràng**
   - Separation of concerns tốt
   - Dễ maintain và extend

2. **SQL Injection Protection**
   - Tất cả queries đều dùng parameterized queries
   - ✅ Security best practice

3. **Transaction Support**
   - Sử dụng transactions cho atomic operations
   - ✅ Data integrity được đảm bảo

4. **Soft Delete Implementation**
   - Sử dụng `isDeleted` flag
   - Trash management tốt

5. **Role-based Access Control**
   - Admin (VT01): Full access
   - Nhân viên (VT02): Limited access
   - Guest mode: Feedback only

## 🚀 Roadmap Cải thiện

### Phase 1: Critical Issues (1-2 tuần)
- [ ] Fix database provider consistency
- [ ] Move credentials to configuration file
- [ ] Implement proper authentication
- [ ] Add password hashing

### Phase 2: High Priority (2-3 tuần)
- [ ] Refactor entities to C# properties
- [ ] Add comprehensive logging
- [ ] Implement centralized error handling
- [ ] Add async/await support

### Phase 3: Medium Priority (1-2 tuần)
- [ ] Add XML documentation comments
- [ ] Create constants for magic strings
- [ ] Implement DTOs
- [ ] Add input validation at UI layer

### Phase 4: Nice to Have (Ongoing)
- [ ] Add unit tests
- [ ] Improve naming conventions
- [ ] Add integration tests
- [ ] Performance optimization

## 🛠️ Tech Stack

- **Language**: C# (.NET Framework hoặc .NET 6+)
- **UI Framework**: Windows Forms
- **Database**: MySQL (hoặc SQL Server - cần chuẩn hóa)
- **Data Access**: ADO.NET (Raw SQL)
- **Architecture**: 3-tier (UI → Controller → DAO → DB)

### Dependencies (Recommended)

```xml
<PackageReference Include="MySql.Data" Version="8.2.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="8.0.0" />
```

## 📦 Setup Instructions

### 1. Clone repository
```bash
git clone https://github.com/vbac-05/KTPM.git
cd KTPM
```

### 2. Setup database
```bash
# Tạo database MySQL
mysql -u root -p
CREATE DATABASE QLTHUOC;
USE QLTHUOC;

# Import schema (nếu có file .sql)
source database_schema.sql;
```

### 3. Configure connection
```bash
# Copy config template
cp App.config.example App.config

# Edit App.config với credentials của bạn
nano App.config
```

### 4. Build project
```bash
dotnet restore
dotnet build
```

### 5. Run application
```bash
dotnet run
```

## 🔐 Security Notes

⚠️ **IMPORTANT**: Dự án hiện tại có nhiều security issues:

1. ❌ Hard-coded credentials trong code
2. ❌ Demo authentication (admin/admin)
3. ❌ Passwords không được hash
4. ❌ Không có rate limiting
5. ❌ Không có audit logging

👉 **Đọc chi tiết**: [TECHNICAL_ANALYSIS.md - Security Analysis](TECHNICAL_ANALYSIS.md#6️⃣-security-analysis)

**KHÔNG deploy production** cho đến khi fix hết các vấn đề security!

## 📝 Contributing

Nếu muốn contribute:

1. Đọc [CODE_REVIEW.md](CODE_REVIEW.md) để hiểu các vấn đề
2. Đọc [QUICK_FIX_GUIDE.md](QUICK_FIX_GUIDE.md) để biết cách fix
3. Pick một issue từ roadmap
4. Tạo branch mới: `git checkout -b feature/your-feature`
5. Commit changes: `git commit -m 'Add feature'`
6. Push: `git push origin feature/your-feature`
7. Tạo Pull Request

## 📧 Contact

Nếu có câu hỏi, tạo issue trong repository hoặc liên hệ team.

---

## 📚 Documentation Index

| Document | Mô tả | Độ ưu tiên |
|----------|-------|-----------|
| [CODE_REVIEW.md](CODE_REVIEW.md) | Code review tổng quan, điểm mạnh/yếu | 📖 Đọc đầu tiên |
| [TECHNICAL_ANALYSIS.md](TECHNICAL_ANALYSIS.md) | Phân tích kỹ thuật chi tiết | 📖 Đọc sau |
| [QUICK_FIX_GUIDE.md](QUICK_FIX_GUIDE.md) | Hướng dẫn sửa lỗi nhanh | 🔧 Action items |
| README.md | Tài liệu này | 📋 Overview |

---

**Last Updated**: 2025-11-03  
**Reviewed By**: GitHub Copilot Agent  
**Version**: 1.0  
**Status**: ⚠️ Development (Not Production Ready)
