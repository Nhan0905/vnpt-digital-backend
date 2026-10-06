using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Domain
{
    public class Store
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;       // Tên điểm giao dịch
        public string Province { get; set; } = string.Empty;   // Mã Tỉnh/TP (VD: 'hcm', 'hn', 'ag')
        public string District { get; set; } = string.Empty;   // Mã Quận/Huyện (VD: 'q1', 'lx')
        public string Address { get; set; } = string.Empty;     // Địa chỉ chi tiết
        public string Phone { get; set; } = string.Empty;       // Số điện thoại
        public string MapEmbed { get; set; } = string.Empty;    // Link iframe Google Map
    }
}