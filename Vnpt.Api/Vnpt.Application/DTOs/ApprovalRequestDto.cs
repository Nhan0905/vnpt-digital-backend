using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Application.DTOs
{
    public class ApprovalRequestDto
    {
        // Nhận giá trị: "Approved" hoặc "Rejected"
        public required string Action { get; set; }

        // Lý do từ chối (có thể bỏ trống nếu duyệt)
        public string? Reason { get; set; }
    }
}