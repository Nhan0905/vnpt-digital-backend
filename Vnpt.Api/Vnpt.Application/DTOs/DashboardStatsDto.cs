using System;
using System.Collections.Generic;
using System.Text;

namespace Vnpt.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalOrders { get; set; } // Tổng số hồ sơ
        public int ApprovedOrders { get; set; } // Số hồ sơ đã duyệt
        public int RejectedOrders { get; set; } // Số hồ sơ bị từ chối
        public int PendingOrders { get; set; } // Số hồ sơ đang chờ duyệt
    }
}