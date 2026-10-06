using System;
using System.Collections.Generic;
using System.Text;

using System;

namespace Vnpt.Domain
{
    public class SupportTicket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; } // Ảnh chụp sự cố (nếu có)
        public string Status { get; set; } = "Chờ tiếp nhận"; // Chờ tiếp nhận, Đang xử lý, Đã hoàn thành
        public string? AssignedTechnician { get; set; } // Nhân viên kỹ thuật được phân công
        public string? ResponseNote { get; set; } // Phản hồi từ nhân viên/admin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}