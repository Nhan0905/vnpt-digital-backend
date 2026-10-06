using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;
using Vnpt.Persistence;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Vnpt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;

        public OrdersController(IOrderService orderService, ApplicationDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin đăng nhập!" });
            }

            Guid customerId = Guid.Parse(userIdClaim.Value);
            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);

            return Ok(new { data = orders });
        }

        // --- ĐÃ SỬA: Đổi thành [FromForm] và thêm xử lý lưu ảnh CCCD ---
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateOrderDTO dto, IFormFile? cccdFront, IFormFile? cccdBack)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin đăng nhập!" });
            }
            Guid customerId = Guid.Parse(userIdClaim.Value);

            // 1. Tạo đơn hàng
            var newOrder = await _orderService.CreateOrderAsync(dto, customerId);

            // 2. Lưu ảnh CCCD (Dùng biến cccdFront và cccdBack bắt được từ hàm)
            if (newOrder != null)
            {
                await SaveUploadedFileAsync(newOrder.Id, cccdFront, "CCCD_Mặt_Trước");
                await SaveUploadedFileAsync(newOrder.Id, cccdBack, "CCCD_Mặt_Sau");
            }

            return Ok(new
            {
                message = "Tạo hồ sơ thành công!",
                data = newOrder
            });
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Guid adminId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;

            var success = await _orderService.ApproveOrderAsync(id, adminId);
            if (!success) return BadRequest(new { message = "Không thể duyệt hồ sơ này!" });

            return Ok(new { message = "Đã duyệt hồ sơ thành công!" });
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectOrderDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Reason)) return BadRequest(new { message = "Phải nhập lý do từ chối!" });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Guid adminId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;

            var success = await _orderService.RejectOrderAsync(id, dto.Reason, adminId);
            if (!success) return BadRequest(new { message = "Không thể từ chối hồ sơ này!" });

            return Ok(new { message = "Đã từ chối hồ sơ!" });
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _orderService.GetPagedOrdersAsync(pageNumber, pageSize, search, status);
            return Ok(new { message = "Lấy danh sách hồ sơ thành công!", data = result });
        }

        // --- ĐÃ SỬA: Đổi thành [FromForm] và thêm xử lý cập nhật ảnh CCCD ---
        [HttpPut("{id}/resubmit")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Resubmit(Guid id, [FromForm] CreateOrderDTO dto, IFormFile? cccdFront, IFormFile? cccdBack)
        {
            var success = await _orderService.ResubmitOrderAsync(id, dto);
            if (!success) return BadRequest(new { message = "Không thể cập nhật hồ sơ này!" });

            // Cập nhật lại ảnh CCCD nếu khách hàng có upload mới
            await SaveUploadedFileAsync(id, cccdFront, "CCCD_Mặt_Trước");
            await SaveUploadedFileAsync(id, cccdBack, "CCCD_Mặt_Sau");

            return Ok(new { message = "Cập nhật và gửi lại hồ sơ thành công!" });
        }

        [HttpGet("stats")]
        [Authorize]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                var orderList = orders.ToList();

                return Ok(new
                {
                    totalOrders = orderList.Count,
                    pendingOrders = orderList.Count(o => o.Status == "Pending"),
                    approvedOrders = orderList.Count(o => o.Status == "Approved"),
                    rejectedOrders = orderList.Count(o => o.Status == "Rejected")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("{id}/documents")]
        [Authorize]
        public async Task<IActionResult> UploadDocument(Guid id, IFormFile file, [FromQuery] string fileType = "CCCD")
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file hợp lệ!" });

            var filePath = await SaveFileToDiskAsync(file);
            if (filePath == null) return BadRequest(new { message = "Định dạng file không hợp lệ!" });

            var success = await _orderService.AddDocumentAsync(id, file.FileName, filePath, fileType, file.Length);
            if (!success) return BadRequest(new { message = "Không thể lưu thông tin tài liệu!" });

            return Ok(new { message = "Upload tài liệu thành công!", filePath = filePath });
        }

        [HttpGet("{id}/logs")]
        [Authorize]
        public async Task<IActionResult> GetLogs(Guid id)
        {
            var logs = await _orderService.GetOrderLogsAsync(id);
            var result = new List<object>();

            foreach (var log in logs)
            {
                string actorName = "Hệ thống";
                var user = await _context.Users.FindAsync(log.UserId);
                if (user != null)
                {
                    actorName = user.FullName;
                }
                else
                {
                    if (log.Action == "Created" || log.Action == "Resubmitted")
                        actorName = "Khách hàng";
                    else
                        actorName = "Nhân viên VNPT";
                }

                result.Add(new
                {
                    createdAt = log.CreatedAt,
                    action = log.Action,
                    reason = log.Reason,
                    actorName = actorName
                });
            }

            return Ok(new { data = result });
        }

        [HttpGet("{id}/documents")]
        [Authorize]
        public async Task<IActionResult> GetDocuments(Guid id)
        {
            var documents = await _orderService.GetDocumentsByOrderIdAsync(id);
            return Ok(new { data = documents });
        }

        // =========================================================================
        // HÀM HỖ TRỢ LƯU FILE (Tái sử dụng để code gọn gàng)
        // =========================================================================
        private async Task<string?> SaveFileToDiskAsync(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension)) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/{uniqueFileName}";
        }

        private async Task SaveUploadedFileAsync(Guid orderId, IFormFile? file, string fileType)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = await SaveFileToDiskAsync(file);
                if (filePath != null)
                {
                    // Tái sử dụng hàm AddDocumentAsync của bạn
                    await _orderService.AddDocumentAsync(orderId, file.FileName, filePath, fileType, file.Length);
                }
            }
        }
    }
}