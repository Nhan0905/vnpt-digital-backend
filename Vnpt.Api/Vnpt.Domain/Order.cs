using System;

namespace Vnpt.Domain
{
    public class Order
    {
        public Guid Id { get; set; }

        // Mã hồ sơ (Ví dụ: HS2026-0001) để khách hàng tra cứu
        public string OrderCode { get; set; } = string.Empty;

        // Khóa ngoại liên kết với Khách hàng (Sẽ lấy từ JWT Token khi đăng nhập)
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Khóa ngoại liên kết với Dịch vụ (Sản phẩm)
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        // --- CÁC TRƯỜNG MỚI BỔ SUNG TỪ FORM FRONTEND ---
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Note { get; set; }

        // Trạng thái hồ sơ: "Pending" (Chờ duyệt), "Approved" (Đã duyệt), "Rejected" (Từ chối)
        public string Status { get; set; } = "Pending";

        // Lý do từ chối (Nếu nhân viên VNPT từ chối hồ sơ)
        public string? RejectReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}