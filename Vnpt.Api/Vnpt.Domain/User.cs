using System;

namespace Vnpt.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";

        // --- CÁC TRƯỜNG THÔNG TIN MỚI CHO KHÁCH HÀNG ---
        public string Gender { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // --- THÔNG TIN QUẢN LÝ ---
        public string CustomerType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string JoinDate { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}