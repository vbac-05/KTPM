// File: Models/NhaCungCap.cs
// Nguồn gốc: chuyển từ Java entities/NhaCungCap.java
// Mapping & ghi chú:
// - Java String            -> C# string
// - Java Boolean isDeleted -> C# bool? (nullable)   // CHANGED: để phản ánh có thể null giống Java Boolean.
// - Tên thuộc tính: đổi từ camelCase (Java) sang PascalCase (chuẩn .NET).
//   => NHỚ đổi DataPropertyName trong các DataGridView/UI/Mapper tương ứng.
//   Ví dụ: "idNCC" -> "IdNCC", "tenNCC" -> "TenNCC", "sdt" -> "Sdt", "diaChi" -> "DiaChi", "isDeleted" -> "IsDeleted".

namespace QLThuocWin.Models
{
    public class NhaCungCap
    {
        // idNCC
        public string IdNCC { get; set; } = string.Empty;

        // tenNCC
        public string TenNCC { get; set; } = string.Empty;

        // sdt
        public string Sdt { get; set; } = string.Empty;

        // diaChi
        public string DiaChi { get; set; } = string.Empty;

        // isDeleted (Java Boolean) -> C# bool?
        public bool? IsDeleted { get; set; }   // CHANGED: nullable

        // --- Constructors tương đương ---
        public NhaCungCap() { }

        public NhaCungCap(string idNcc, string tenNcc, string sdt, string diaChi, bool? isDeleted = null)
        {
            IdNCC = idNcc;
            TenNCC = tenNcc;
            Sdt = sdt;
            DiaChi = diaChi;
            IsDeleted = isDeleted;
        }

        public override string ToString()
            => $"{IdNCC} - {TenNCC} ({Sdt}) | {DiaChi}";
    }
}
