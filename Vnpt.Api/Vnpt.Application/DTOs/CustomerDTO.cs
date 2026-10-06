using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Application.DTOs
{
    public class CustomerDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string IdentityNo { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}