using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Domain
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty; // Mã dịch vụ
        public string Name { get; set; } = string.Empty; // Tên dịch vụ
        public string Category { get; set; } = string.Empty; // Loại dịch vụ
        public decimal Price { get; set; } // Giá cước
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // Link hình ảnh

        // THÊM TRƯỜNG NÀY ĐỂ LƯU MÔ TẢ CHI TIẾT:
        public string? Description { get; set; }
    }
}