# ⚡ Quick Fix Guide - KTPM Project

> Hướng dẫn nhanh để fix các vấn đề quan trọng nhất trong project

## 📋 Mục lục
1. [Fix Database Provider Mismatch](#1-fix-database-provider-mismatch)
2. [Move Credentials to Config](#2-move-credentials-to-config)
3. [Fix Demo Authentication](#3-fix-demo-authentication)
4. [Refactor Entities to Properties](#4-refactor-entities-to-properties)
5. [Add Password Hashing](#5-add-password-hashing)

---

## 1️⃣ Fix Database Provider Mismatch

### 🔴 Vấn đề
- `DBConnection.cs` dùng SQL Server
- `ThuocDAO.cs` và các DAO khác dùng MySQL
- Code không thể compile

### ✅ Giải pháp

**Bước 1**: Quyết định database nào sẽ dùng

**Option A: Dùng MySQL** (Khuyến nghị nếu đã có database MySQL)

**File: connectDB/DBConnection.cs**
```csharp
// TRƯỚC (SQL Server)
using Microsoft.Data.SqlClient;

private static readonly string CONNECTION_STRING =
    "Server=localhost,1433;" +
    "Database=QLTHUOC;" +
    "User Id=sa;" +
    "Password=123123;" +
    "Encrypt=False;";

public static SqlConnection GetConnection()
{
    var conn = new SqlConnection(CONNECTION_STRING);
    conn.Open();
    return conn;
}
```

```csharp
// SAU (MySQL)
using MySql.Data.MySqlClient;

private static readonly string CONNECTION_STRING =
    "Server=localhost;" +
    "Port=3306;" +
    "Database=QLTHUOC;" +
    "User Id=root;" +
    "Password=your_password;";

public static MySqlConnection GetConnection()
{
    try
    {
        var conn = new MySqlConnection(CONNECTION_STRING);
        conn.Open();
        return conn;
    }
    catch (MySqlException ex)
    {
        Console.Error.WriteLine("Không thể kết nối MySQL:");
        Console.Error.WriteLine(ex);
        throw;
    }
}
```

**File: connectDB/DBCloseHelper.cs**
```csharp
// TRƯỚC
using Microsoft.Data.SqlClient;
public static void Close(SqlDataReader reader) { }
public static void Close(SqlCommand cmd) { }
public static void Close(SqlConnection conn) { }

// SAU
using MySql.Data.MySqlClient;
public static void Close(MySqlDataReader reader) { }
public static void Close(MySqlCommand cmd) { }
public static void Close(MySqlConnection conn) { }
```

**Bước 2**: Install MySQL NuGet package
```bash
dotnet add package MySql.Data --version 8.2.0
```

**Option B: Dùng SQL Server**

Nếu chọn SQL Server, cần update TẤT CẢ DAO files:
- `MySqlConnection` → `SqlConnection`
- `MySqlCommand` → `SqlCommand`
- `MySqlDataReader` → `SqlDataReader`
- `MySqlDbType` → `SqlDbType`
- `MySqlException` → `SqlException`

---

## 2️⃣ Move Credentials to Config

### 🔴 Vấn đề
- Connection string với password hard-coded trong code
- Security vulnerability nghiêm trọng

### ✅ Giải pháp

**Bước 1**: Tạo App.config file

**File: App.config** (tạo mới ở root của project)
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <!-- MySQL -->
    <add name="QLTHUOC" 
         connectionString="Server=localhost;Port=3306;Database=QLTHUOC;User Id=root;Password=YOUR_PASSWORD_HERE;"/>
    
    <!-- Hoặc SQL Server -->
    <!-- <add name="QLTHUOC" 
         connectionString="Server=localhost;Database=QLTHUOC;User Id=sa;Password=YOUR_PASSWORD_HERE;Encrypt=False;"/> -->
  </connectionStrings>
  
  <appSettings>
    <add key="Environment" value="Development"/>
    <add key="MaxLoginAttempts" value="3"/>
  </appSettings>
</configuration>
```

**Bước 2**: Install System.Configuration package
```bash
dotnet add package System.Configuration.ConfigurationManager --version 8.0.0
```

**Bước 3**: Update DBConnection.cs

```csharp
using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace connectDB
{
    public static class DBConnection
    {
        // ✅ Đọc từ config file
        private static readonly string CONNECTION_STRING = 
            ConfigurationManager.ConnectionStrings["QLTHUOC"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            try
            {
                var conn = new MySqlConnection(CONNECTION_STRING);
                conn.Open();
                return conn;
            }
            catch (MySqlException ex)
            {
                Console.Error.WriteLine("Không thể kết nối database:");
                Console.Error.WriteLine(ex);
                throw;
            }
        }
    }
}
```

**Bước 4**: Add App.config to .gitignore

```gitignore
# Database config - DO NOT commit
App.config
```

**Bước 5**: Tạo App.config.example (để team biết format)

**File: App.config.example**
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="QLTHUOC" 
         connectionString="Server=localhost;Port=3306;Database=QLTHUOC;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;"/>
  </connectionStrings>
</configuration>
```

---

## 3️⃣ Fix Demo Authentication

### 🔴 Vấn đề
```csharp
// LoginForm.cs - Hard-coded demo login
bool ok = (username == "admin" && password == "admin")
       || (username == "nv" && password == "nv");
```

### ✅ Giải pháp

**Bước 1**: Tạo TaiKhoanDAO.cs (nếu chưa có)

**File: dao/TaiKhoanDAO.cs**
```csharp
using System;
using MySql.Data.MySqlClient;
using Entities;

namespace DAO
{
    public class TaiKhoanDAO
    {
        /// <summary>
        /// Lấy tài khoản theo username
        /// </summary>
        public TaiKhoan GetByUsername(string username)
        {
            string sql = "SELECT * FROM TaiKhoan WHERE username = @username";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TaiKhoan
                                {
                                    Username = reader["username"].ToString(),
                                    Password = reader["password"].ToString(),
                                    RoleId = reader["roleId"].ToString(),
                                    IsLocked = reader["isLocked"] != DBNull.Value && (bool)reader["isLocked"]
                                };
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error getting account: {ex.Message}");
                throw;
            }
            
            return null;
        }
    }
}
```

**Bước 2**: Tạo LoginController.cs (nếu chưa có hoặc update)

**File: controller/LoginController.cs**
```csharp
using System;
using DAO;
using Entities;

namespace Controller
{
    public class LoginController
    {
        private readonly TaiKhoanDAO _taiKhoanDAO;
        
        public LoginController()
        {
            _taiKhoanDAO = new TaiKhoanDAO();
        }
        
        /// <summary>
        /// Xác thực người dùng
        /// </summary>
        public TaiKhoan Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Username và password không được để trống");
            }
            
            // Lấy tài khoản từ database
            var account = _taiKhoanDAO.GetByUsername(username.Trim());
            
            if (account == null)
            {
                return null; // Tài khoản không tồn tại
            }
            
            if (account.IsLocked)
            {
                throw new Exception("Tài khoản đã bị khóa");
            }
            
            // Kiểm tra password
            // TODO: Implement password hashing (xem section 5)
            if (account.Password == password)
            {
                return account;
            }
            
            return null; // Sai password
        }
    }
}
```

**Bước 3**: Update LoginForm.cs

**File: UI/LoginForm.cs**
```csharp
using System;
using System.Windows.Forms;
using Controller;
using Utils;

namespace QLThuocWin.UI
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnGuest;
        private int _loginAttempts = 0;
        
        // ... constructor code ...
        
        private void PerformLogin()
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;
            
            // 1. Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageDialog.ShowWarning(this, 
                    "Vui lòng nhập đầy đủ Username/Password", 
                    "Thiếu thông tin");
                return;
            }
            
            // 2. Check login attempts
            if (_loginAttempts >= 3)
            {
                MessageDialog.ShowError(this, 
                    "Tài khoản tạm thời bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau.", 
                    "Tài khoản bị khóa");
                return;
            }
            
            try
            {
                // 3. Authenticate
                var loginController = new LoginController();
                var account = loginController.Authenticate(username, password);
                
                if (account == null)
                {
                    _loginAttempts++;
                    MessageDialog.ShowError(this, 
                        $"Sai username hoặc password. Còn {3 - _loginAttempts} lần thử.", 
                        "Lỗi đăng nhập");
                    return;
                }
                
                // 4. Reset attempts on success
                _loginAttempts = 0;
                
                // 5. Open main form
                var mainForm = new MainForm(account.RoleId, account.Username);
                mainForm.Show();
                this.Hide();
                mainForm.FormClosed += (s, e) => this.Close();
            }
            catch (Exception ex)
            {
                MessageDialog.ShowError(this, 
                    $"Lỗi đăng nhập: {ex.Message}", 
                    "Lỗi");
            }
        }
        
        // ... rest of code ...
    }
}
```

**Bước 4**: Tạo test accounts trong database

```sql
-- Tạo bảng TaiKhoan nếu chưa có
CREATE TABLE IF NOT EXISTS TaiKhoan (
    username NVARCHAR(50) PRIMARY KEY,
    password NVARCHAR(255) NOT NULL,
    roleId NVARCHAR(10) NOT NULL,
    isLocked BIT DEFAULT 0,
    createdAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Tạo tài khoản test (TODO: Hash passwords sau)
INSERT INTO TaiKhoan (username, password, roleId) VALUES 
('admin', 'admin123', 'VT01'),
('nhanvien', 'nv123', 'VT02');
```

---

## 4️⃣ Refactor Entities to Properties

### 🔴 Vấn đề
Entities dùng Java-style getter/setter thay vì C# properties

### ✅ Giải pháp

**Example: Refactor Thuoc.cs**

**TRƯỚC (Java style)**:
```csharp
[Table("Thuoc")]
public class Thuoc
{
    [Key]
    [Column("idThuoc")]
    [MaxLength(10)]
    [Required]
    private string idThuoc;
    
    public string GetIdThuoc()
    {
        return idThuoc;
    }
    
    public void SetIdThuoc(string idThuoc)
    {
        this.idThuoc = idThuoc;
    }
    
    // ... repeat for 14 more fields ...
}
```

**SAU (C# style)**:
```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("Thuoc")]
    public class Thuoc
    {
        [Key]
        [Column("idThuoc")]
        [MaxLength(10)]
        [Required]
        public string IdThuoc { get; set; } = string.Empty;
        
        [Column("tenThuoc")]
        [MaxLength(255)]
        [Required]
        public string TenThuoc { get; set; } = string.Empty;
        
        [Column("hinhAnh")]
        public byte[]? HinhAnh { get; set; }
        
        [Column("thanhPhan")]
        [MaxLength(255)]
        public string? ThanhPhan { get; set; }
        
        [Column("donViTinh")]
        [MaxLength(255)]
        [Required]
        public string DonViTinh { get; set; } = string.Empty;
        
        [Column("danhMuc")]
        [MaxLength(255)]
        [Required]
        public string DanhMuc { get; set; } = string.Empty;
        
        [Column("xuatXu")]
        [MaxLength(10)]
        [Required]
        public string XuatXu { get; set; } = string.Empty;
        
        [Column("soLuongTon")]
        [Required]
        public int SoLuongTon { get; set; }
        
        [Column("giaNhap")]
        [Required]
        public decimal GiaNhap { get; set; }  // ✅ Changed to decimal
        
        [Column("donGia")]
        [Required]
        public decimal DonGia { get; set; }  // ✅ Changed to decimal
        
        [Column("hanSuDung")]
        [Required]
        public DateTime HanSuDung { get; set; }
        
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }
        
        // Constructor mặc định
        public Thuoc() { }
    }
}
```

**Update all usages**:

```csharp
// TRƯỚC
string maThuoc = thuoc.GetIdThuoc();
thuoc.SetTenThuoc("Paracetamol");

// SAU
string maThuoc = thuoc.IdThuoc;
thuoc.TenThuoc = "Paracetamol";
```

**Find and Replace (Visual Studio)**:
1. Find: `thuoc.GetIdThuoc()` → Replace: `thuoc.IdThuoc`
2. Find: `thuoc.SetIdThuoc\((.*?)\)` → Replace: `thuoc.IdThuoc = $1`
3. Repeat for all properties

---

## 5️⃣ Add Password Hashing

### 🔴 Vấn đề
Passwords được lưu plaintext trong database

### ✅ Giải pháp

**Bước 1**: Install BCrypt.Net package
```bash
dotnet add package BCrypt.Net-Next --version 4.0.3
```

**Bước 2**: Tạo PasswordHasher utility

**File: ultis/PasswordHasher.cs**
```csharp
using BCrypt.Net;

namespace Utils
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Hash password sử dụng BCrypt
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, BCrypt.GenerateSalt(12));
        }
        
        /// <summary>
        /// Verify password với hash
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }
    }
}
```

**Bước 3**: Update LoginController

```csharp
using Utils;

public class LoginController
{
    public TaiKhoan Authenticate(string username, string password)
    {
        var account = _taiKhoanDAO.GetByUsername(username.Trim());
        
        if (account == null)
            return null;
        
        if (account.IsLocked)
            throw new Exception("Tài khoản đã bị khóa");
        
        // ✅ Verify hashed password
        if (PasswordHasher.VerifyPassword(password, account.Password))
        {
            return account;
        }
        
        return null;
    }
}
```

**Bước 4**: Hash existing passwords trong database

**Script: HashPasswords.sql**
```sql
-- Backup bảng trước
CREATE TABLE TaiKhoan_Backup AS SELECT * FROM TaiKhoan;

-- Update schema: tăng length của password field
ALTER TABLE TaiKhoan MODIFY COLUMN password NVARCHAR(255);
```

**Script: HashPasswords.cs** (Run once để hash passwords)
```csharp
using System;
using Utils;
using DAO;

public class HashPasswordsScript
{
    public static void Main()
    {
        var taiKhoanDAO = new TaiKhoanDAO();
        var accounts = taiKhoanDAO.GetAll();
        
        foreach (var account in accounts)
        {
            // Skip nếu đã hash (length > 50)
            if (account.Password.Length > 50)
                continue;
            
            // Hash password
            var hashedPassword = PasswordHasher.HashPassword(account.Password);
            
            // Update database
            account.Password = hashedPassword;
            taiKhoanDAO.Update(account);
            
            Console.WriteLine($"Hashed password for: {account.Username}");
        }
        
        Console.WriteLine("Done!");
    }
}
```

**Bước 5**: Update any registration/password change code

```csharp
// Khi tạo tài khoản mới
public bool CreateAccount(string username, string password, string roleId)
{
    var account = new TaiKhoan
    {
        Username = username,
        Password = PasswordHasher.HashPassword(password),  // ✅ Hash password
        RoleId = roleId
    };
    
    return _taiKhoanDAO.Insert(account);
}

// Khi đổi password
public bool ChangePassword(string username, string oldPassword, string newPassword)
{
    var account = _taiKhoanDAO.GetByUsername(username);
    
    // Verify old password
    if (!PasswordHasher.VerifyPassword(oldPassword, account.Password))
    {
        throw new Exception("Mật khẩu cũ không đúng");
    }
    
    // Update với password mới (hashed)
    account.Password = PasswordHasher.HashPassword(newPassword);
    return _taiKhoanDAO.Update(account);
}
```

---

## ✅ Checklist

Sau khi làm xong 5 bước trên, check lại:

- [ ] **Database provider consistent** (MySQL hoặc SQL Server, không mix)
- [ ] **Connection string trong App.config** (không còn trong code)
- [ ] **App.config trong .gitignore** (không commit credentials)
- [ ] **Authentication dùng database** (không còn hard-coded)
- [ ] **Login attempts tracking** (max 3 lần)
- [ ] **Entities dùng C# properties** (không còn getter/setter)
- [ ] **Money fields dùng decimal** (không còn double)
- [ ] **Passwords được hash** (không còn plaintext)
- [ ] **BCrypt.Net installed** (cho password hashing)
- [ ] **Test login với accounts thật** (từ database)

---

## 🚀 Next Steps

Sau khi hoàn thành các fixes trên, nên làm tiếp:

1. **Add Logging** (Serilog hoặc NLog)
2. **Add Async/Await** (cho database operations)
3. **Add Unit Tests** (xUnit hoặc NUnit)
4. **Add Input Validation** (ở UI layer)
5. **Improve Error Handling** (centralized exception handling)

Xem chi tiết trong `CODE_REVIEW.md` và `TECHNICAL_ANALYSIS.md`.

---

**Last Updated**: 2025-11-03  
**Priority**: 🔴 CRITICAL - Làm ngay
