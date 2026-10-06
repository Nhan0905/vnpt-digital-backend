using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Application.DTOs
{
    public class AdminUpdateUserDto
    {
        public string CustomerType { get; set; } = string.Empty; // VD: Cá nhân, Doanh nghiệp, VIP
        public string Status { get; set; } = string.Empty;       // VD: Đang hoạt động, Bị khóa, Chờ duyệt
        public string Role { get; set; } = string.Empty;         // VD: Customer, Staff, Admin
    }
}