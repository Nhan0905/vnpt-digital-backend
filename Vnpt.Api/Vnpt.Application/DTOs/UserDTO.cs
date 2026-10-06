using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Application.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
}