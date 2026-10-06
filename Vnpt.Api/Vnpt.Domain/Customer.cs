using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Domain
{
    // Kế thừa từ BaseEntity nên sẽ tự động có cột Id và IsDeleted
    public class Customer : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string IdentityNo { get; set; } = string.Empty; // Số CCCD/MST
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}