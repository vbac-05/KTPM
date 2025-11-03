# 📋 CODE REVIEW - Hệ thống Quản lý Nhà Thuốc (KTPM)

## 🎯 Tổng quan dự án

**Loại dự án**: Ứng dụng quản lý nhà thuốc (Pharmacy Management System)  
**Ngôn ngữ**: C# (Windows Forms Application)  
**Cơ sở dữ liệu**: MySQL (với một số file còn tham chiếu SQL Server)  
**Kiến trúc**: 3-tier Architecture (Entity, DAO, Controller, UI)  
**Tổng số file**: 71 file C#  
**Tổng số dòng code**: ~15,522 dòng

---

## 🏗️ Kiến trúc hệ thống

### Cấu trúc thư mục:
```
KTPM/
├── Entities/        # 15 entity classes (Thuoc, HoaDon, NhanVien, etc.)
├── dao/            # 11 DAO classes (Data Access Objects)
├── controller/      # 9 Controller classes
├── UI/             # 36 UI forms và panels
├── connectDB/      # 2 database connection utilities
└── ultis/          # 5 utility classes
```

---

## ✅ Điểm mạnh

### 1. **Kiến trúc phân tầng rõ ràng**
- ✅ Tách biệt rõ ràng giữa Entity, DAO, Controller và UI
- ✅ Tuân thủ nguyên tắc Single Responsibility
- ✅ Dễ dàng bảo trì và mở rộng

### 2. **Entity Layer (Models)**
- ✅ Sử dụng Data Annotations đầy đủ (`[Table]`, `[Key]`, `[Column]`, `[MaxLength]`, `[Required]`)
- ✅ Các entity được thiết kế tốt với nullable types phù hợp
- ✅ Comment chi tiết về mapping với database
- ✅ Constructor mặc định đầy đủ

**Ví dụ tốt** (Thuoc.cs):
```csharp
[Table("Thuoc")]
public class Thuoc
{
    [Key]
    [Column("idThuoc")]
    [MaxLength(10)]
    [Required]
    private string idThuoc;
    // ... các field khác
}
```

### 3. **DAO Layer (Data Access)**
- ✅ Sử dụng Parameterized Queries để phòng SQL Injection
- ✅ Proper resource management với `using` statements
- ✅ Transaction support cho các thao tác phức tạp
- ✅ Error handling tương đối tốt
- ✅ Implement soft delete (isDeleted)

**Ví dụ tốt** (ThuocDAO.cs):
```csharp
using (MySqlConnection conn = DBConnection.GetConnection())
{
    conn.Open();
    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
    {
        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = idThuoc;
        // ... xử lý
    }
}
```

### 4. **Utility Classes**
- ✅ Validator class với các hàm kiểm tra dữ liệu đầu vào
- ✅ MessageDialog để chuẩn hóa hiển thị thông báo
- ✅ ImageHelper, DateHelper, ExportHelper cho các tác vụ phổ biến

### 5. **Controller Layer**
- ✅ Xử lý business logic trước khi gọi DAO
- ✅ Error handling với output parameters (`out string errorMsg`)
- ✅ Validation logic tập trung

### 6. **UI Layer**
- ✅ Phân tách theo chức năng (Panel cho mỗi entity)
- ✅ Role-based access control (Admin vs Nhân viên)
- ✅ Trash management (Thùng rác) cho soft delete
- ✅ Guest mode cho feedback

---

## ⚠️ Vấn đề và khuyến nghị cải thiện

### 🔴 VẤN ĐỀ NGHIÊM TRỌNG (Critical)

#### 1. **Inconsistent Database Provider**
**Vấn đề**: 
- Một số file sử dụng `Microsoft.Data.SqlClient` (SQL Server)
- Một số file sử dụng `MySql.Data.MySqlClient` (MySQL)
- Gây confusion và có thể lỗi runtime

**File bị ảnh hưởng**:
- `connectDB/DBConnection.cs` → SQL Server
- `connectDB/DBCloseHelper.cs` → SQL Server
- `dao/*.cs` → MySQL

**Khuyến nghị**:
```csharp
// CHỌN MỘT TRONG HAI:
// Option 1: Chuẩn hóa toàn bộ sang MySQL
using MySql.Data.MySqlClient;

// Option 2: Chuẩn hóa toàn bộ sang SQL Server
using Microsoft.Data.SqlClient;

// Cập nhật lại DBConnection.cs và tất cả DAO
```

#### 2. **Hard-coded Database Credentials**
**Vấn đề**: Connection string chứa credentials được hard-code trong code
```csharp
// connectDB/DBConnection.cs
private static readonly string CONNECTION_STRING =
    "Server=localhost,1433;" +
    "Database=QLTHUOC;" +
    "User Id=sa;" +           // ❌ Hard-coded
    "Password=123123;" +      // ❌ NGUY HIỂM!
    "Encrypt=False;";
```

**Rủi ro**:
- 🔴 Bảo mật kém
- 🔴 Credentials bị expose trong source code
- 🔴 Không thể thay đổi môi trường dễ dàng

**Khuyến nghị**:
```csharp
// Sử dụng Configuration file
// App.config hoặc appsettings.json
<connectionStrings>
    <add name="QLTHUOC" 
         connectionString="Server=localhost;Database=QLTHUOC;User Id=sa;Password=***;"/>
</connectionStrings>

// Đọc từ config
private static readonly string CONNECTION_STRING = 
    ConfigurationManager.ConnectionStrings["QLTHUOC"].ConnectionString;
```

#### 3. **Lỗi logic nghiêm trọng trong ThuocDAO.cs**
**Vấn đề**: Hàm `DeleteForever` ban đầu xóa từ bảng sai
```csharp
// ThuocDAO.cs line 356 (ĐÃ SỬA)
// LỖI CŨ: string sql = "DELETE FROM PhieuNhap WHERE idThuoc = @idThuoc";
// ĐÚNG: string sql = "DELETE FROM Thuoc WHERE idThuoc = @idThuoc";
```

**Khuyến nghị**: Đã được sửa trong code, cần test kỹ lưỡng

#### 4. **Không có Input Validation ở UI Layer**
**Vấn đề**: 
- Validation chủ yếu ở Controller
- UI có thể gửi dữ liệu không hợp lệ
- Trải nghiệm người dùng kém

**Khuyến nghị**:
```csharp
// Thêm validation ngay tại UI
private void btnSave_Click(object sender, EventArgs e)
{
    // Validate trước khi gọi Controller
    if (string.IsNullOrWhiteSpace(txtTenThuoc.Text))
    {
        MessageDialog.ShowWarning(this, "Vui lòng nhập tên thuốc", "Cảnh báo");
        txtTenThuoc.Focus();
        return;
    }
    
    if (!Validator.IsDouble(txtGiaNhap.Text))
    {
        MessageDialog.ShowWarning(this, "Giá nhập phải là số", "Cảnh báo");
        txtGiaNhap.Focus();
        return;
    }
    
    // ... tiếp tục xử lý
}
```

### 🟡 VẤN ĐỀ QUAN TRỌNG (High Priority)

#### 5. **Entity Classes Sử Dụng Private Fields + Getters/Setters**
**Vấn đề**: 
- Code theo style Java (private fields + getter/setter methods)
- Không theo C# convention
- Khó tích hợp với các framework .NET

**Ví dụ hiện tại**:
```csharp
// Thuoc.cs - JAVA STYLE
private string idThuoc;

public string GetIdThuoc() { return idThuoc; }
public void SetIdThuoc(string idThuoc) { this.idThuoc = idThuoc; }
```

**Khuyến nghị** (C# Style):
```csharp
// C# CONVENTION
[Key]
[Column("idThuoc")]
[MaxLength(10)]
[Required]
public string IdThuoc { get; set; } = string.Empty;
```

**Lợi ích**:
- ✅ Ngắn gọn hơn (1 dòng thay vì 10 dòng)
- ✅ Tương thích với Entity Framework
- ✅ Tương thích với JSON serialization
- ✅ Theo chuẩn C#

**Action**: Cần refactor toàn bộ 15 entity classes

#### 6. **Không sử dụng async/await**
**Vấn đề**:
- Tất cả database operations đều synchronous
- UI bị "đơ" khi thực hiện operations dài
- Trải nghiệm người dùng kém

**Khuyến nghị**:
```csharp
// TRƯỚC (Synchronous)
public List<Thuoc> GetAllThuoc()
{
    // ... blocking operation
}

// SAU (Asynchronous)
public async Task<List<Thuoc>> GetAllThuocAsync()
{
    using (MySqlConnection conn = DBConnection.GetConnection())
    {
        await conn.OpenAsync();
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                // ... map data
            }
        }
    }
}
```

#### 7. **Thiếu Logging**
**Vấn đề**:
- Chỉ có `Console.WriteLine()` để debug
- Không có structured logging
- Khó troubleshoot production issues

**Khuyến nghị**:
```csharp
// Sử dụng logging framework như Serilog hoặc NLog
using Serilog;

public class ThuocDAO
{
    private static readonly ILogger _logger = Log.ForContext<ThuocDAO>();
    
    public List<Thuoc> GetAllThuoc()
    {
        try
        {
            _logger.Information("Bắt đầu lấy danh sách thuốc");
            // ... code
            _logger.Information("Lấy thành công {Count} thuốc", list.Count);
            return list;
        }
        catch (MySqlException ex)
        {
            _logger.Error(ex, "Lỗi khi lấy danh sách thuốc");
            throw;
        }
    }
}
```

#### 8. **Error Handling chưa đồng nhất**
**Vấn đề**:
- Một số nơi catch exception và return false
- Một số nơi throw exception
- Một số nơi catch và in Console.WriteLine
- Không có centralized error handling

**Khuyến nghị**:
```csharp
// Tạo custom exception classes
public class DatabaseException : Exception
{
    public DatabaseException(string message, Exception innerException) 
        : base(message, innerException) { }
}

// Centralized error handling
public class GlobalExceptionHandler
{
    public static void Handle(Exception ex)
    {
        _logger.Error(ex, "Unhandled exception");
        MessageDialog.ShowError(null, 
            $"Đã xảy ra lỗi: {ex.Message}", 
            "Lỗi hệ thống");
    }
}

// Trong Application.Run
Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
Application.ThreadException += (s, e) => GlobalExceptionHandler.Handle(e.Exception);
```

### 🟢 VẤN ĐỀ TRUNG BÌNH (Medium Priority)

#### 9. **Code Comments Chưa Chuẩn**
**Vấn đề**:
- Mix giữa tiếng Việt và tiếng Anh
- Có nhiều TODO comment chưa resolve
- Thiếu XML documentation comments

**Khuyến nghị**:
```csharp
/// <summary>
/// Lấy danh sách tất cả các thuốc chưa bị xóa mềm.
/// </summary>
/// <returns>Danh sách thuốc</returns>
/// <exception cref="DatabaseException">Khi có lỗi truy vấn database</exception>
public List<Thuoc> GetAllThuoc()
{
    // Implementation
}
```

#### 10. **MainForm.cs Có Một Số Vấn Đề**
**Vấn đề**:
```csharp
// Line 79: TODO comment
// TODO: Nếu project của bạn vẫn là ThuocControl, đổi dòng dưới về new ThuocControl().
var thuocPanel = new ThuocPanel();

// Line 110: Constructor không khớp
var main = new MainForm(roleId); // TODO: nếu constructor khác, đổi ở đây
```

**Khuyến nghị**: Xác định và sử dụng constructor đúng

#### 11. **LoginForm.cs Demo Authentication**
**Vấn đề**:
```csharp
// Line 95: Hard-coded demo authentication
bool ok = (username == "admin" && password == "admin")
       || (username == "nv" && password == "nv");
```

**Rủi ro**:
- 🔴 Security vulnerability
- 🔴 Không thể deploy production

**Khuyến nghị**: Implement proper authentication
```csharp
private void PerformLogin()
{
    var username = txtUsername.Text.Trim();
    var password = txtPassword.Text;
    
    // Validate input
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        MessageDialog.ShowWarning(this, "Vui lòng nhập đầy đủ thông tin", "Cảnh báo");
        return;
    }
    
    // Authenticate với database thật
    try
    {
        var loginController = new LoginController();
        var account = loginController.Authenticate(username, password);
        
        if (account == null)
        {
            MessageDialog.ShowError(this, "Sai username hoặc password", "Lỗi đăng nhập");
            return;
        }
        
        // Mở MainForm với roleId từ database
        var main = new MainForm(account.RoleId, account.Username);
        main.Show();
        this.Hide();
        main.FormClosed += (s, e) => this.Close();
    }
    catch (Exception ex)
    {
        MessageDialog.ShowError(this, $"Lỗi đăng nhập: {ex.Message}", "Lỗi");
    }
}
```

#### 12. **Không có Unit Tests**
**Vấn đề**: 
- Không có test projects
- Không thể verify logic correctness
- Rủi ro cao khi refactor

**Khuyến nghị**:
```csharp
// Tạo test project với xUnit hoặc NUnit
[Fact]
public void GiamSoLuong_ValidInput_ReturnsTrue()
{
    // Arrange
    var dao = new ThuocDAO();
    var initialQuantity = 100;
    
    // Act
    var result = dao.GiamSoLuong("T001", 10);
    
    // Assert
    Assert.True(result);
}

[Fact]
public void GiamSoLuong_InsufficientStock_ReturnsFalse()
{
    // Arrange
    var dao = new ThuocDAO();
    
    // Act
    var result = dao.GiamSoLuong("T001", 1000);
    
    // Assert
    Assert.False(result);
}
```

#### 13. **DBCloseHelper Không Cần Thiết**
**Vấn đề**: 
- C# có `using` statement tự động dispose
- DBCloseHelper class là redundant

**Khuyến nghị**: Xóa DBCloseHelper và chỉ dùng `using`
```csharp
// KHÔNG CẦN DBCloseHelper
using (MySqlConnection conn = DBConnection.GetConnection())
using (MySqlCommand cmd = new MySqlCommand(sql, conn))
using (MySqlDataReader reader = cmd.ExecuteReader())
{
    // Tự động dispose khi ra khỏi scope
}
```

#### 14. **Không có Data Transfer Objects (DTOs)**
**Vấn đề**:
- Truyền Entity trực tiếp giữa các layer
- Entity bị expose ra UI layer
- Khó control dữ liệu trả về

**Khuyến nghị**:
```csharp
// Tạo DTOs cho UI
public class ThuocDisplayDto
{
    public string MaThuoc { get; set; }
    public string TenThuoc { get; set; }
    public string DonViTinh { get; set; }
    public int SoLuongTon { get; set; }
    public string GiaBanFormatted { get; set; } // "1,000,000 VNĐ"
    public string HanSuDungFormatted { get; set; } // "31/12/2025"
}

// Mapping trong Controller
public List<ThuocDisplayDto> GetThuocForDisplay()
{
    var thuocs = _thuocDao.GetAllThuoc();
    return thuocs.Select(t => new ThuocDisplayDto
    {
        MaThuoc = t.GetIdThuoc(),
        TenThuoc = t.GetTenThuoc(),
        // ... mapping
    }).ToList();
}
```

#### 15. **Magic Strings và Magic Numbers**
**Vấn đề**:
```csharp
// Hard-coded roles
if (roleId == "VT01") // ❌ Magic string
{
    // Admin tabs
}
else if (roleId == "VT02") // ❌ Magic string
{
    // Employee tabs
}
```

**Khuyến nghị**:
```csharp
// Tạo constants class
public static class RoleConstants
{
    public const string ADMIN = "VT01";
    public const string EMPLOYEE = "VT02";
    public const string GUEST = "VT03";
}

// Sử dụng
if (roleId == RoleConstants.ADMIN)
{
    // Admin tabs
}
```

#### 16. **Naming Convention Không Đồng Nhất**
**Vấn đề**:
- Một số nơi dùng tiếng Việt không dấu (ThuocDAO)
- Một số nơi dùng tiếng Việt có dấu (giảm số lượng)
- Method names tiếng Việt + tiếng Anh

**Khuyến nghị**: Chọn một convention và áp dụng đồng nhất
```csharp
// OPTION 1: Toàn bộ tiếng Anh (Khuyến nghị cho dự án lớn)
public bool DecreaseStock(string medicineId, int quantity)

// OPTION 2: Tiếng Việt không dấu
public bool GiamSoLuongThuoc(string maThuoc, int soLuong)

// ❌ TRÁNH: Mix lẫn lộn
public bool GiamSoLuong(string idThuoc, int soLuong)
```

---

## 🎯 Khuyến nghị triển khai theo thứ tự ưu tiên

### Phase 1: Critical Issues (1-2 tuần)
1. ✅ **Chuẩn hóa database provider** (MySQL hoặc SQL Server)
2. ✅ **Move credentials ra configuration file**
3. ✅ **Implement proper authentication** (thay thế hard-coded login)
4. ✅ **Fix DeleteForever bug** (đã sửa, cần test)

### Phase 2: High Priority (2-3 tuần)
5. ✅ **Refactor Entities sang C# Properties**
6. ✅ **Add comprehensive logging**
7. ✅ **Implement centralized error handling**
8. ✅ **Add async/await support**

### Phase 3: Medium Priority (1-2 tuần)
9. ✅ **Add XML documentation comments**
10. ✅ **Create constants for magic strings**
11. ✅ **Implement DTOs**
12. ✅ **Add input validation at UI layer**
13. ✅ **Remove DBCloseHelper**

### Phase 4: Nice to Have (Ongoing)
14. ✅ **Add unit tests**
15. ✅ **Improve naming conventions**
16. ✅ **Add integration tests**
17. ✅ **Performance optimization**

---

## 📊 Code Quality Metrics

| Tiêu chí | Đánh giá | Điểm |
|----------|----------|------|
| **Architecture** | ✅ Tốt | 8/10 |
| **Code Organization** | ✅ Tốt | 8/10 |
| **Security** | ⚠️ Cần cải thiện | 4/10 |
| **Error Handling** | ⚠️ Trung bình | 6/10 |
| **Performance** | ⚠️ Trung bình | 6/10 |
| **Maintainability** | ✅ Tốt | 7/10 |
| **Testing** | ❌ Không có | 0/10 |
| **Documentation** | ⚠️ Trung bình | 5/10 |
| **Code Style** | ⚠️ Không đồng nhất | 5/10 |

**Tổng điểm**: **6.1/10** - Cần cải thiện

---

## 🔐 Security Checklist

- ❌ **Credentials hardcoded** → Cần fix ngay
- ✅ **SQL Injection protected** (Parameterized queries)
- ❌ **No password hashing** → Cần implement
- ⚠️ **No authentication system** → Đang dùng demo code
- ⚠️ **No authorization checks** → Chỉ kiểm tra role ở UI
- ❌ **No audit logging** → Không track user actions
- ⚠️ **Encrypt=False** in connection string → Không mã hóa connection

---

## 📝 Kết luận

### Điểm mạnh tổng thể:
1. ✅ **Kiến trúc 3-tier rõ ràng và hợp lý**
2. ✅ **Code được tổ chức tốt theo chức năng**
3. ✅ **Sử dụng parameterized queries đúng cách**
4. ✅ **Transaction support cho operations phức tạp**
5. ✅ **Soft delete implementation tốt**

### Điểm cần cải thiện:
1. 🔴 **Security**: Hard-coded credentials, demo authentication
2. 🔴 **Consistency**: Mixed database providers (MySQL vs SQL Server)
3. 🟡 **Code Style**: Java-style entities thay vì C# convention
4. 🟡 **Testing**: Không có unit tests
5. 🟡 **Documentation**: Thiếu XML comments chuẩn
6. 🟡 **Performance**: Thiếu async/await
7. 🟡 **Logging**: Chỉ có Console.WriteLine

### Đánh giá chung:
Đây là một dự án có **nền tảng kiến trúc tốt** nhưng còn nhiều vấn đề về **security, code style và testing**. Code được tổ chức khá tốt và dễ đọc, tuy nhiên cần **refactoring đáng kể** để đạt production-ready quality.

### Khuyến nghị:
1. **Ưu tiên cao nhất**: Fix các vấn đề security (credentials, authentication)
2. **Tiếp theo**: Chuẩn hóa database provider và code style
3. **Dài hạn**: Thêm tests, logging và async support

---

## 📞 Liên hệ

Nếu có câu hỏi về code review này, vui lòng tạo issue hoặc discussion trong repository.

---

**Review Date**: 2025-11-03  
**Reviewer**: GitHub Copilot Agent  
**Version**: 1.0
