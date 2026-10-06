using System;

namespace Vnpt.Application.DTOs
{
    public class RegisterDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // -- CÁC TRƯỜNG THÊM MỚI --
        public string Gender { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string Role { get; set; } = "Customer"; // Đăng ký bên ngoài mặc định là Khách hàng
    }
}