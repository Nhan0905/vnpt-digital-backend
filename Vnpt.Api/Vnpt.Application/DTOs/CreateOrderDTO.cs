using System;
using System.Collections.Generic;
using System.Text;
using System;

namespace Vnpt.Application.DTOs
{
    public class CreateOrderDTO
    {
        public Guid ProductId { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
    }
}