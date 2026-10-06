using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vnpt.Application.Interfaces;

namespace Vnpt.Api.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize] // Yêu cầu phải đăng nhập mới được xem thống kê
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IExcelService _excelService; // Đã thêm biến này

        // Đã cập nhật constructor để nhận IExcelService
        public DashboardController(IDashboardRepository dashboardRepository, IExcelService excelService)
        {
            _dashboardRepository = dashboardRepository;
            _excelService = excelService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _dashboardRepository.GetStatisticsAsync();
            return Ok(new
            {
                message = "Lấy dữ liệu thống kê thành công",
                data = stats
            });
        }

        // API mới dùng để xuất file Excel
        [HttpGet("export")]
        public async Task<IActionResult> ExportDashboard()
        {
            // 1. Lấy dữ liệu thống kê từ Database
            var stats = await _dashboardRepository.GetStatisticsAsync();

            // 2. Gọi hàm vẽ ra file Excel
            var fileContent = _excelService.GenerateDashboardReport(stats);

            // 3. Trả file về cho trình duyệt/Postman tải xuống
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoThongKe.xlsx");
        }
    }
}