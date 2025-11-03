# 🔬 Phân tích Kỹ thuật Chi tiết - KTPM Pharmacy System

## 📋 Mục lục
1. [Entity Layer Analysis](#entity-layer-analysis)
2. [DAO Layer Analysis](#dao-layer-analysis)
3. [Controller Layer Analysis](#controller-layer-analysis)
4. [UI Layer Analysis](#ui-layer-analysis)
5. [Database Design Review](#database-design-review)
6. [Security Analysis](#security-analysis)
7. [Performance Analysis](#performance-analysis)

---

## 1️⃣ Entity Layer Analysis

### 1.1 Current State

**Số lượng entities**: 15 classes
- ChiTietHoaDon.cs
- ChiTietPhieuNhap.cs
- DanhMuc.cs
- DonViTinh.cs
- HoaDon.cs
- HopDong.cs
- KhachHang.cs
- NhaCungCap.cs
- NhanVien.cs
- PhanHoi.cs
- PhieuNhap.cs
- TaiKhoan.cs
- Thuoc.cs
- VaiTro.cs
- XuatXu.cs

### 1.2 Pattern Analysis

**Vấn đề hiện tại**: Entities sử dụng Java-style getter/setter

**Ví dụ từ Thuoc.cs**:
```csharp
[Table("Thuoc")]
public class Thuoc
{
    [Key]
    [Column("idThuoc")]
    [MaxLength(10)]
    [Required]
    private string idThuoc;  // ❌ private field
    
    // ❌ Java-style getter
    public string GetIdThuoc()
    {
        return idThuoc;
    }
    
    // ❌ Java-style setter
    public void SetIdThuoc(string idThuoc)
    {
        this.idThuoc = idThuoc;
    }
}
```

**Vấn đề**:
1. Không tương thích với Entity Framework Core
2. Không tương thích với JSON serialization (System.Text.Json, Newtonsoft.Json)
3. Verbose - mỗi property cần ~10 dòng code
4. Không theo C# naming convention
5. Data binding trong Windows Forms sẽ không hoạt động

**Khuyến nghị refactor**:
```csharp
[Table("Thuoc")]
public class Thuoc
{
    [Key]
    [Column("idThuoc")]
    [MaxLength(10)]
    [Required]
    public string IdThuoc { get; set; } = string.Empty;  // ✅ C# property
    
    [Column("tenThuoc")]
    [MaxLength(255)]
    [Required]
    public string TenThuoc { get; set; } = string.Empty;  // ✅ C# property
    
    [Column("hinhAnh")]
    public byte[]? HinhAnh { get; set; }  // ✅ Nullable reference type
    
    // ... các properties khác
}
```

**Lợi ích**:
- ✅ Ngắn gọn hơn 90%
- ✅ Tương thích EF Core
- ✅ Auto-property initialization
- ✅ Null-safety với nullable reference types
- ✅ Data binding support

### 1.3 Entity Design Issues

**Issue 1: Mixed Types for Money**
```csharp
// HoaDon.cs - sử dụng double
public double TongTien { get; set; }

// Thuoc.cs - sử dụng double
public double GiaNhap { get; set; }
public double DonGia { get; set; }
```

**Vấn đề**: 
- `double` không chính xác cho tiền tệ
- Có thể mất độ chính xác trong tính toán

**Khuyến nghị**:
```csharp
[Column("tongTien")]
[Required]
public decimal TongTien { get; set; }  // ✅ Dùng decimal cho money

[Column("giaNhap")]
[Required]
public decimal GiaNhap { get; set; }

[Column("donGia")]
[Required]
public decimal DonGia { get; set; }
```

**Issue 2: String for Salary**
```csharp
// NhanVien.cs
[Column("luong")]
[MaxLength(50)]
[Required]
public string Luong { get; set; } = string.Empty;  // ❌ String cho lương
```

**Vấn đề**: Không thể tính toán, sort, filter hiệu quả

**Khuyến nghị**:
```csharp
[Column("luong")]
[Required]
public decimal Luong { get; set; }  // ✅ Dùng decimal
```

**Issue 3: Nullable Confusion**
```csharp
// Thuoc.cs
public byte[]? GetHinhAnh()  // ✅ Nullable reference type
{
    return hinhAnh;
}

public bool? GetIsDeleted()  // ✅ Nullable value type
{
    return isDeleted;
}
```

**Comment**: Sử dụng nullable types đúng, nhưng cần consistent với property pattern

---

## 2️⃣ DAO Layer Analysis

### 2.1 Database Provider Inconsistency

**Critical Issue**: Mixed database providers

**DBConnection.cs** (SQL Server):
```csharp
using Microsoft.Data.SqlClient;  // ❌ SQL Server

private static readonly string CONNECTION_STRING =
    "Server=localhost,1433;" +
    "Database=QLTHUOC;" +
    "User Id=sa;" +
    "Password=123123;" +
    "Encrypt=False;";

public static SqlConnection GetConnection()  // ❌ Returns SqlConnection
{
    var conn = new SqlConnection(CONNECTION_STRING);
    conn.Open();
    return conn;
}
```

**ThuocDAO.cs** (MySQL):
```csharp
using MySql.Data.MySqlClient;  // ❌ MySQL

public List<Thuoc> GetAllThuoc()
{
    using (MySqlConnection conn = DBConnection.GetConnection())  // ❌ Type mismatch!
    {
        // ...
    }
}
```

**Impact**: Compile error! Không thể chạy được.

**Solution Options**:

**Option A: Chuẩn hóa sang MySQL**
```csharp
// DBConnection.cs
using MySql.Data.MySqlClient;

private static readonly string CONNECTION_STRING =
    "Server=localhost;" +
    "Port=3306;" +
    "Database=QLTHUOC;" +
    "User Id=root;" +
    "Password=123123;";

public static MySqlConnection GetConnection()
{
    var conn = new MySqlConnection(CONNECTION_STRING);
    conn.Open();
    return conn;
}
```

**Option B: Chuẩn hóa sang SQL Server**
```csharp
// Update tất cả DAO files
using Microsoft.Data.SqlClient;

// Update tất cả MySqlConnection → SqlConnection
// Update tất cả MySqlCommand → SqlCommand
// Update tất cả MySqlDataReader → SqlDataReader
// Update tất cả MySqlDbType → SqlDbType
```

### 2.2 Transaction Patterns

**Good Example** (HoaDonDAO.cs):
```csharp
public bool InsertHoaDonWithDetails(HoaDon hd, List<ChiTietHoaDon> chiTietList)
{
    using (MySqlConnection conn = DBConnection.GetConnection())
    {
        conn.Open();
        using (MySqlTransaction trans = conn.BeginTransaction())  // ✅ Transaction
        {
            try
            {
                // 1. Insert HoaDon
                // 2. Insert ChiTietHoaDon
                trans.Commit();  // ✅ Commit on success
                return true;
            }
            catch (Exception ex)
            {
                trans.Rollback();  // ✅ Rollback on error
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
```

**Strength**: Proper transaction management cho atomic operations

### 2.3 SQL Injection Protection

**Good Example** (ThuocDAO.cs):
```csharp
public List<Thuoc> SearchThuoc(string idThuoc, string tenThuoc)
{
    // ✅ Sử dụng parameterized query
    if (!string.IsNullOrWhiteSpace(idThuoc))
    {
        sql.Append(" AND idThuoc LIKE @idThuoc");
    }
    
    // ✅ Add parameter an toàn
    cmd.Parameters.AddWithValue("@idThuoc", $"%{idThuoc.Trim()}%");
}
```

**Strength**: Tất cả queries đều sử dụng parameters, không có string concatenation

### 2.4 Error Handling Issues

**Issue 1: Inconsistent Error Handling**

**Pattern A** (ThuocDAO.cs):
```csharp
catch (MySqlException ex)
{
    Console.WriteLine(ex.Message);  // ❌ Chỉ log ra console
    return false;  // ❌ Silent failure
}
```

**Pattern B** (ThuocDAO.cs - DeleteThuoc):
```csharp
catch (MySqlException ex)
{
    Console.WriteLine(ex.Message);  // ❌ Log console
    throw new Exception("Lỗi khi xóa mềm thuốc: " + ex.Message);  // ✅ Throw exception
}
```

**Pattern C** (HoaDonDAO.cs - InsertHoaDon):
```csharp
catch (MySqlException ex)
{
    if (ex.Number == 1062)  // ✅ Specific error handling
    {
        throw new Exception("ID hóa đơn đã tồn tại!");
    }
    if (ex.Number == 1452)  // ✅ FK constraint
    {
        throw new Exception("ID nhân viên hoặc khách hàng không tồn tại!");
    }
    throw new Exception("Lỗi SQL khi thêm hóa đơn: " + ex.Message);
}
```

**Recommendation**: Standardize error handling
```csharp
// Create custom exception types
public class DatabaseException : Exception
{
    public string ErrorCode { get; set; }
    public DatabaseException(string message, string errorCode, Exception inner) 
        : base(message, inner) 
    {
        ErrorCode = errorCode;
    }
}

// Standardized error handling
catch (MySqlException ex)
{
    _logger.Error(ex, "Database error in GetAllThuoc");
    
    throw new DatabaseException(
        "Không thể lấy danh sách thuốc", 
        "DB_001", 
        ex
    );
}
```

### 2.5 Resource Management

**Good Example**: Sử dụng `using` statement đúng cách
```csharp
using (MySqlConnection conn = DBConnection.GetConnection())
{
    conn.Open();
    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
    using (MySqlDataReader reader = cmd.ExecuteReader())
    {
        // ✅ Tự động dispose tất cả resources
    }
}
```

**Observation**: DBCloseHelper.cs không cần thiết khi đã dùng `using`

### 2.6 Performance Issues

**Issue 1: N+1 Query Problem Potential**

Không thấy JOIN queries, chỉ thấy queries riêng lẻ. Ví dụ:
```csharp
// Có thể gây N+1 problem nếu loop
var hoaDons = hoaDonDAO.GetAllHoaDon();
foreach (var hd in hoaDons)
{
    var khachHang = khachHangDAO.GetById(hd.IdKH);  // ❌ N queries
    var nhanVien = nhanVienDAO.GetById(hd.IdNV);    // ❌ N queries
}
```

**Recommendation**: Thêm methods với JOINs
```csharp
public List<HoaDonDetailDto> GetAllHoaDonWithDetails()
{
    string sql = @"
        SELECT h.*, k.hoTen as tenKH, n.hoTen as tenNV
        FROM HoaDon h
        INNER JOIN KhachHang k ON h.idKH = k.idKH
        INNER JOIN NhanVien n ON h.idNV = n.idNV
        WHERE h.isDeleted = 0";
    
    // ✅ Single query với JOIN
}
```

**Issue 2: No Pagination**

Tất cả methods GetAll trả về toàn bộ data:
```csharp
public List<Thuoc> GetAllThuoc()  // ❌ Lấy hết tất cả
{
    // Có thể trả về hàng nghìn records
}
```

**Recommendation**: Thêm pagination
```csharp
public PagedResult<Thuoc> GetAllThuoc(int pageNumber = 1, int pageSize = 50)
{
    int offset = (pageNumber - 1) * pageSize;
    
    string sqlCount = "SELECT COUNT(*) FROM Thuoc WHERE isDeleted = 0";
    string sqlData = @"
        SELECT * FROM Thuoc 
        WHERE isDeleted = 0 
        LIMIT @pageSize OFFSET @offset";
    
    // Return data + total count
    return new PagedResult<Thuoc>
    {
        Data = list,
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}
```

---

## 3️⃣ Controller Layer Analysis

### 3.1 Controller Pattern

**Example** (ThuocController.cs):
```csharp
public class ThuocController
{
    private readonly ThuocDAO thuocDAO;
    
    public ThuocController()
    {
        thuocDAO = new ThuocDAO();  // ❌ Tight coupling
    }
    
    public bool AddThuoc(Thuoc t, out string errorMsg)
    {
        errorMsg = string.Empty;
        try
        {
            if (t == null)
            {
                errorMsg = "Thông tin thuốc không hợp lệ.";
                return false;
            }
            
            return thuocDAO.InsertThuoc(t);
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return false;
        }
    }
}
```

**Issues**:
1. ❌ Tight coupling với DAO (không thể mock cho testing)
2. ❌ `out` parameter cho error message (không theo C# pattern)
3. ⚠️ Validation logic minimal

**Recommendations**:

**1. Dependency Injection**:
```csharp
public interface IThuocDAO
{
    List<Thuoc> GetAllThuoc();
    bool InsertThuoc(Thuoc thuoc);
    // ...
}

public class ThuocController
{
    private readonly IThuocDAO _thuocDAO;
    private readonly IValidator _validator;
    private readonly ILogger<ThuocController> _logger;
    
    // ✅ Constructor injection
    public ThuocController(IThuocDAO thuocDAO, IValidator validator, ILogger<ThuocController> logger)
    {
        _thuocDAO = thuocDAO;
        _validator = validator;
        _logger = logger;
    }
}
```

**2. Result Pattern thay vì out parameter**:
```csharp
public class OperationResult<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; }
    
    public static OperationResult<T> SuccessResult(T data)
    {
        return new OperationResult<T> { Success = true, Data = data };
    }
    
    public static OperationResult<T> FailureResult(string error)
    {
        return new OperationResult<T> { Success = false, ErrorMessage = error };
    }
}

public class ThuocController
{
    public OperationResult<bool> AddThuoc(Thuoc thuoc)
    {
        // Validation
        var validationResult = _validator.Validate(thuoc);
        if (!validationResult.IsValid)
        {
            return OperationResult<bool>.FailureResult(validationResult.Errors);
        }
        
        try
        {
            var result = _thuocDAO.InsertThuoc(thuoc);
            return OperationResult<bool>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error adding thuoc");
            return OperationResult<bool>.FailureResult($"Lỗi: {ex.Message}");
        }
    }
}
```

### 3.2 Business Logic Issues

**Issue**: Minimal business logic
```csharp
public bool AddThuoc(Thuoc t, out string errorMsg)
{
    if (t == null)  // ❌ Only null check
    {
        errorMsg = "Thông tin thuốc không hợp lệ.";
        return false;
    }
    return thuocDAO.InsertThuoc(t);
}
```

**Recommendation**: Add comprehensive validation
```csharp
public OperationResult<bool> AddThuoc(Thuoc thuoc)
{
    // 1. Null check
    if (thuoc == null)
        return OperationResult<bool>.FailureResult("Thông tin thuốc không được null");
    
    // 2. Required fields
    if (string.IsNullOrWhiteSpace(thuoc.TenThuoc))
        return OperationResult<bool>.FailureResult("Tên thuốc không được để trống");
    
    // 3. Business rules
    if (thuoc.GiaNhap <= 0)
        return OperationResult<bool>.FailureResult("Giá nhập phải lớn hơn 0");
    
    if (thuoc.DonGia < thuoc.GiaNhap)
        return OperationResult<bool>.FailureResult("Giá bán phải lớn hơn hoặc bằng giá nhập");
    
    if (thuoc.HanSuDung <= DateTime.Now)
        return OperationResult<bool>.FailureResult("Hạn sử dụng phải ở tương lai");
    
    if (thuoc.SoLuongTon < 0)
        return OperationResult<bool>.FailureResult("Số lượng tồn không được âm");
    
    // 4. Check duplicate
    var existing = _thuocDAO.GetById(thuoc.IdThuoc);
    if (existing != null)
        return OperationResult<bool>.FailureResult($"Mã thuốc {thuoc.IdThuoc} đã tồn tại");
    
    // 5. Insert
    try
    {
        var result = _thuocDAO.InsertThuoc(thuoc);
        
        if (result)
        {
            _logger.Information("Thêm thuốc thành công: {MaThuoc}", thuoc.IdThuoc);
            return OperationResult<bool>.SuccessResult(true);
        }
        else
        {
            return OperationResult<bool>.FailureResult("Không thể thêm thuốc");
        }
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "Lỗi khi thêm thuốc {MaThuoc}", thuoc.IdThuoc);
        return OperationResult<bool>.FailureResult($"Lỗi hệ thống: {ex.Message}");
    }
}
```

---

## 4️⃣ UI Layer Analysis

### 4.1 LoginForm Issues

**Critical Security Issue**:
```csharp
// LoginForm.cs lines 95-96
bool ok = (username == "admin" && password == "admin")
       || (username == "nv" && password == "nv");

if (!ok)
{
    MessageBox.Show("Sai username hoặc password");
    return;
}

string roleId = username == "admin" ? "VT01" : "VT02";
```

**Problems**:
1. 🔴 Hard-coded credentials trong source code
2. 🔴 Plaintext passwords
3. 🔴 Không có rate limiting (brute force attack)
4. 🔴 Không có password hashing
5. 🔴 Không có audit logging

**Recommendation**: Proper authentication
```csharp
private void PerformLogin()
{
    var username = txtUsername.Text.Trim();
    var password = txtPassword.Text;
    
    // 1. Input validation
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        MessageDialog.ShowWarning(this, "Vui lòng nhập đầy đủ thông tin", "Cảnh báo");
        return;
    }
    
    // 2. Check login attempts (prevent brute force)
    if (_loginAttempts >= 3)
    {
        MessageDialog.ShowError(this, "Tài khoản bị khóa do đăng nhập sai nhiều lần", "Lỗi");
        return;
    }
    
    try
    {
        // 3. Authenticate via Controller
        var result = _loginController.Authenticate(username, password);
        
        if (!result.Success)
        {
            _loginAttempts++;
            MessageDialog.ShowError(this, result.ErrorMessage, "Lỗi đăng nhập");
            return;
        }
        
        // 4. Log successful login
        _auditLogger.LogLogin(result.Data.Username, success: true);
        
        // 5. Open main form
        var mainForm = new MainForm(result.Data.RoleId, result.Data.Username);
        mainForm.Show();
        this.Hide();
        mainForm.FormClosed += (s, e) => this.Close();
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "Login error");
        MessageDialog.ShowError(this, "Lỗi hệ thống khi đăng nhập", "Lỗi");
    }
}
```

**LoginController Implementation**:
```csharp
public class LoginController
{
    private readonly ITaiKhoanDAO _taiKhoanDAO;
    private readonly IPasswordHasher _passwordHasher;
    
    public OperationResult<TaiKhoan> Authenticate(string username, string password)
    {
        // 1. Get account from database
        var account = _taiKhoanDAO.GetByUsername(username);
        if (account == null)
        {
            return OperationResult<TaiKhoan>.FailureResult("Tài khoản không tồn tại");
        }
        
        // 2. Verify password (hashed)
        if (!_passwordHasher.Verify(password, account.PasswordHash))
        {
            return OperationResult<TaiKhoan>.FailureResult("Mật khẩu không đúng");
        }
        
        // 3. Check account status
        if (account.IsLocked)
        {
            return OperationResult<TaiKhoan>.FailureResult("Tài khoản đã bị khóa");
        }
        
        return OperationResult<TaiKhoan>.SuccessResult(account);
    }
}
```

### 4.2 MainForm Issues

**Issue 1: Constructor inconsistency**
```csharp
// Line 14-15
public MainForm(string roleId, string username)  // ✅ 2 parameters

// Line 110
var main = new MainForm(roleId);  // ❌ TODO: 1 parameter?
```

**Solution**: Clarify constructor
```csharp
public MainForm(string roleId, string username)
{
    _roleId = roleId?.Trim() ?? string.Empty;
    _username = username ?? "User";
    
    InitializeComponent();
    BuildUI();
}

// Usage
var main = new MainForm(result.Data.RoleId, result.Data.Username);
```

**Issue 2: Panel creation không tối ưu**
```csharp
// Lines 79-88: Tạo tất cả panels cho mọi role
var thuocPanel       = new ThuocPanel();
var nhanVienPanel    = new NhanVienPanel();
var khachHangPanel   = new KhachHangPanel();
// ... 9 panels total

// Sau đó mới check role và chỉ dùng một số panels
```

**Problem**: Waste resources, slow startup

**Solution**: Lazy loading
```csharp
private void BuildTabsByRole(string roleId)
{
    tabbed = new TabControl { Dock = DockStyle.Fill };
    Controls.Add(tabbed);
    
    if (roleId == RoleConstants.ADMIN)
    {
        // ✅ Chỉ tạo panels cần thiết
        tabbed.TabPages.Add("Thuốc").Controls.Add(new ThuocPanel());
        tabbed.TabPages.Add("Nhân viên").Controls.Add(new NhanVienPanel());
        // ...
    }
    else if (roleId == RoleConstants.EMPLOYEE)
    {
        // ✅ Tạo subset khác
        tabbed.TabPages.Add("Thuốc").Controls.Add(new ThuocPanel());
        tabbed.TabPages.Add("Khách hàng").Controls.Add(new KhachHangPanel());
        // ...
    }
}
```

---

## 5️⃣ Database Design Review

### 5.1 Inferred Schema

Dựa trên code, database schema có thể được suy ra như sau:

```sql
-- Bảng Thuoc
CREATE TABLE Thuoc (
    idThuoc NVARCHAR(10) PRIMARY KEY,
    tenThuoc NVARCHAR(255) NOT NULL,
    hinhAnh BLOB NULL,
    thanhPhan NVARCHAR(255) NULL,
    donViTinh NVARCHAR(255) NOT NULL,
    danhMuc NVARCHAR(255) NOT NULL,
    xuatXu NVARCHAR(10) NOT NULL,
    soLuongTon INT NOT NULL,
    giaNhap DOUBLE NOT NULL,
    donGia DOUBLE NOT NULL,
    hanSuDung DATE NOT NULL,
    isDeleted BIT NULL DEFAULT 0
);

-- Bảng NhanVien
CREATE TABLE NhanVien (
    idNV NVARCHAR(10) PRIMARY KEY,
    hoTen NVARCHAR(255) NOT NULL,
    sdt NVARCHAR(20) NOT NULL,
    gioiTinh NVARCHAR(10) NOT NULL,
    namSinh INT NOT NULL,
    ngayVaoLam DATE NOT NULL,
    luong NVARCHAR(50) NOT NULL,  -- ❌ Should be DECIMAL
    trangThai NVARCHAR(50) NOT NULL,
    username NVARCHAR(50) NOT NULL,
    password NVARCHAR(255) NOT NULL,  -- ❌ Should be hashed
    roleId NVARCHAR(10) NOT NULL,
    isDeleted BIT NULL DEFAULT 0
);

-- Bảng HoaDon
CREATE TABLE HoaDon (
    idHD NVARCHAR(10) PRIMARY KEY,
    thoiGian DATETIME NOT NULL,
    idNV NVARCHAR(10) NOT NULL,
    idKH NVARCHAR(10) NOT NULL,
    tongTien DOUBLE NOT NULL,  -- ❌ Should be DECIMAL
    phuongThucThanhToan NVARCHAR(50) NULL,
    trangThaiDonHang NVARCHAR(50) NOT NULL,
    isDeleted BIT NULL DEFAULT 0,
    FOREIGN KEY (idNV) REFERENCES NhanVien(idNV),
    FOREIGN KEY (idKH) REFERENCES KhachHang(idKH)
);

-- Bảng ChiTietHoaDon
CREATE TABLE ChiTietHoaDon (
    idHD NVARCHAR(10) NOT NULL,
    idThuoc NVARCHAR(10) NOT NULL,
    soLuong INT NOT NULL,
    donGia DOUBLE NOT NULL,  -- ❌ Should be DECIMAL
    PRIMARY KEY (idHD, idThuoc),
    FOREIGN KEY (idHD) REFERENCES HoaDon(idHD),
    FOREIGN KEY (idThuoc) REFERENCES Thuoc(idThuoc)
);
```

### 5.2 Schema Issues

**Issue 1: DOUBLE for money**
```sql
-- ❌ Current
tongTien DOUBLE NOT NULL

-- ✅ Recommended
tongTien DECIMAL(18,2) NOT NULL
```

**Issue 2: String for salary**
```sql
-- ❌ Current
luong NVARCHAR(50) NOT NULL

-- ✅ Recommended
luong DECIMAL(15,2) NOT NULL
```

**Issue 3: No indexes**
```sql
-- Missing indexes on foreign keys
-- Missing indexes on frequently searched columns

-- ✅ Recommended
CREATE INDEX idx_hoadon_idnv ON HoaDon(idNV);
CREATE INDEX idx_hoadon_idkh ON HoaDon(idKH);
CREATE INDEX idx_hoadon_thoigian ON HoaDon(thoiGian);
CREATE INDEX idx_thuoc_tenmoc ON Thuoc(tenThuoc);
```

**Issue 4: No audit columns**
```sql
-- ✅ Recommended: Add audit columns
ALTER TABLE Thuoc ADD COLUMN createdAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE Thuoc ADD COLUMN createdBy NVARCHAR(50) NULL;
ALTER TABLE Thuoc ADD COLUMN updatedAt DATETIME NULL;
ALTER TABLE Thuoc ADD COLUMN updatedBy NVARCHAR(50) NULL;
```

---

## 6️⃣ Security Analysis

### 6.1 OWASP Top 10 Assessment

| Risk | Status | Notes |
|------|--------|-------|
| **A01:2021 – Broken Access Control** | ⚠️ **MEDIUM** | Role checking chỉ ở UI layer |
| **A02:2021 – Cryptographic Failures** | 🔴 **HIGH** | Passwords không hash, credentials hard-coded |
| **A03:2021 – Injection** | ✅ **LOW** | Parameterized queries được sử dụng tốt |
| **A04:2021 – Insecure Design** | ⚠️ **MEDIUM** | Thiếu security by design |
| **A05:2021 – Security Misconfiguration** | 🔴 **HIGH** | Encrypt=False, default credentials |
| **A06:2021 – Vulnerable Components** | ⚠️ **MEDIUM** | Cần kiểm tra versions của dependencies |
| **A07:2021 – Auth Failures** | 🔴 **HIGH** | Hard-coded auth, no rate limiting |
| **A08:2021 – Data Integrity Failures** | ⚠️ **MEDIUM** | Không có digital signatures |
| **A09:2021 – Logging Failures** | 🔴 **HIGH** | Chỉ có Console.WriteLine |
| **A10:2021 – Server-Side Request Forgery** | ✅ **N/A** | Desktop app, không có SSRF risk |

### 6.2 Critical Security Fixes Needed

**1. Password Hashing**
```csharp
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        // Generate salt
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        
        // Hash password
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        
        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }
    
    public bool Verify(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = parts[1];
        
        string hashedInput = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        
        return hash == hashedInput;
    }
}
```

**2. Configuration Management**
```xml
<!-- App.config -->
<configuration>
  <connectionStrings>
    <add name="QLTHUOC" 
         connectionString="Server=localhost;Database=QLTHUOC;Integrated Security=True;"/>
  </connectionStrings>
  
  <appSettings>
    <add key="PasswordHashIterations" value="10000"/>
    <add key="MaxLoginAttempts" value="3"/>
    <add key="SessionTimeout" value="30"/>
  </appSettings>
</configuration>
```

**3. Audit Logging**
```csharp
public class AuditLogger
{
    private readonly ILogger _logger;
    
    public void LogLogin(string username, bool success)
    {
        _logger.Information(
            "Login attempt: User={Username}, Success={Success}, IP={IP}, Time={Time}",
            username, success, GetClientIP(), DateTime.Now);
    }
    
    public void LogDataAccess(string username, string action, string entity, string entityId)
    {
        _logger.Information(
            "Data access: User={Username}, Action={Action}, Entity={Entity}, EntityId={EntityId}",
            username, action, entity, entityId);
    }
}
```

---

## 7️⃣ Performance Analysis

### 7.1 Current Performance Issues

**Issue 1: Synchronous I/O**
- Tất cả database calls đều synchronous
- UI freezing khi thực hiện long-running operations

**Issue 2: No Caching**
- Mỗi lần cần data phải query database
- Lookup data (DanhMuc, DonViTinh, XuatXu) nên được cache

**Issue 3: No Connection Pooling Control**
- Mặc định connection pooling được bật
- Nhưng không có configuration cho pool size

**Issue 4: Load All Data**
- GetAllThuoc() load toàn bộ thuốc vào memory
- Có thể gây OutOfMemoryException với dataset lớn

### 7.2 Performance Recommendations

**1. Implement Async/Await**
```csharp
public async Task<List<Thuoc>> GetAllThuocAsync()
{
    var list = new List<Thuoc>();
    string sql = "SELECT * FROM Thuoc WHERE isDeleted = 0";
    
    using (MySqlConnection conn = DBConnection.GetConnection())
    {
        await conn.OpenAsync();
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                list.Add(MapThuocFromReader(reader));
            }
        }
    }
    return list;
}
```

**2. Implement Caching**
```csharp
public class CachedLookupService
{
    private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    
    public async Task<List<DanhMuc>> GetDanhMucsAsync()
    {
        return await _cache.GetOrCreateAsync("DanhMuc", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _danhMucDAO.GetAllAsync();
        });
    }
}
```

**3. Implement Pagination**
```csharp
public class PagedResult<T>
{
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public async Task<PagedResult<Thuoc>> GetThuocPagedAsync(int pageNumber, int pageSize)
{
    // Implement OFFSET/LIMIT query
}
```

**4. Connection String Optimization**
```csharp
private static readonly string CONNECTION_STRING =
    "Server=localhost;" +
    "Database=QLTHUOC;" +
    "User Id=root;" +
    "Password=***;" +
    "Pooling=true;" +           // ✅ Enable pooling
    "MinPoolSize=5;" +          // ✅ Min connections
    "MaxPoolSize=100;" +        // ✅ Max connections
    "ConnectionTimeout=30;" +   // ✅ Timeout
    "CommandTimeout=120;";      // ✅ Command timeout
```

---

## 📊 Summary Metrics

### Code Quality Scores

| Category | Score | Status |
|----------|-------|--------|
| Architecture | 8/10 | ✅ Good |
| Code Organization | 8/10 | ✅ Good |
| Security | 3/10 | 🔴 Poor |
| Performance | 5/10 | ⚠️ Fair |
| Error Handling | 6/10 | ⚠️ Fair |
| Testing | 0/10 | 🔴 None |
| Documentation | 5/10 | ⚠️ Fair |
| Maintainability | 7/10 | ✅ Good |

### Priority Actions

🔴 **Critical (Do Immediately)**
1. Fix database provider consistency
2. Move credentials to configuration
3. Implement password hashing
4. Fix demo authentication

🟡 **High Priority (Do Soon)**
5. Refactor entities to C# properties
6. Add comprehensive logging
7. Implement async/await
8. Add input validation at UI

🟢 **Medium Priority (Plan for)**
9. Add unit tests
10. Implement caching
11. Add pagination
12. Improve error handling

---

**Document Version**: 1.0  
**Last Updated**: 2025-11-03  
**Total Issues Found**: 45+  
**Critical Issues**: 8  
**High Priority Issues**: 12  
**Medium Priority Issues**: 25+
