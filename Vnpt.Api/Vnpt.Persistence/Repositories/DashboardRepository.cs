using Microsoft.EntityFrameworkCore;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;

namespace Vnpt.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetStatisticsAsync()
        {
            // 1. Đếm tổng số hồ sơ trong hệ thống
            var total = await _context.Orders.CountAsync();

            // 2. Đếm số lượng hồ sơ đã được duyệt
            var approved = await _context.ApprovalLogs.CountAsync(a => a.Action == "Approved");

            // 3. Đếm số lượng hồ sơ bị từ chối
            var rejected = await _context.ApprovalLogs.CountAsync(a => a.Action == "Rejected");

            // 4. Số hồ sơ chờ duyệt (Tổng trừ đi số đã xử lý)
            var pending = total - (approved + rejected);
            if (pending < 0) pending = 0; // Đảm bảo số liệu không bị âm

            return new DashboardStatsDto
            {
                TotalOrders = total,
                ApprovedOrders = approved,
                RejectedOrders = rejected,
                PendingOrders = pending
            };
        }
    }
}