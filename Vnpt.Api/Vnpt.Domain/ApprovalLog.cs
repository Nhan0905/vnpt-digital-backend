using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Domain.Entities
{
    public class ApprovalLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Khóa ngoại liên kết với hồ sơ nào đang được duyệt
        public Guid OrderId { get; set; }

        // Người thực hiện hành động (Sẽ lấy từ JWT Token sau này)
        public Guid UserId { get; set; }

        // Hành động: "Approved" (Đã duyệt) hoặc "Rejected" (Từ chối)
        public required string Action { get; set; }

        // Lý do từ chối (Nếu duyệt thì có thể để trống)
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}