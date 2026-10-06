using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Application.DTOs
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectReason { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }

        // --- THÊM 2 TRƯỜNG NÀY ĐỂ HỨNG FILE ẢNH TỪ REACT ---
      
    }
}