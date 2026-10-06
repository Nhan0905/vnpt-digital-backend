using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Domain.Entities
{
    public class OrderDocument
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public string FileName { get; set; } = string.Empty; // Tên file gốc (VD: CCCD_MatTruoc.jpg)
        public string FilePath { get; set; } = string.Empty; // Đường dẫn lưu trên server
        public string FileType { get; set; } = string.Empty; // Loại file (VD: .jpg, .pdf)
        public long FileSize { get; set; } // Dung lượng file (Bytes)

        public int Version { get; set; } = 1; // Quản lý phiên bản (như đề cương yêu cầu)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}