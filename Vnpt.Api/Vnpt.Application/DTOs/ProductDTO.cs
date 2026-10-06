using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Application.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty; // Mã dịch vụ
        public string Name { get; set; } = string.Empty; // Tên dịch vụ
        public string Category { get; set; } = string.Empty; // Loại dịch vụ (Internet, Chữ ký số...)
        public decimal Price { get; set; } // Giá cước
        public bool IsActive { get; set; } // Trạng thái
        public string ImageUrl { get; set; } = string.Empty; // Link hình ảnh
        public string? Description { get; set; }
    }
}
