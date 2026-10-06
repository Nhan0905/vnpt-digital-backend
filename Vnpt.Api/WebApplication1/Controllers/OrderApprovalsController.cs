using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;

namespace Vnpt.Api.Controllers
{
    [Route("api/orders/{orderId}/approval")]
    [ApiController]
    [Authorize] // Cực kỳ quan trọng: Bắt buộc phải có JWT Token mới được vào
    public class OrderApprovalsController : ControllerBase
    {
        private readonly IApprovalService _approvalService;

        public OrderApprovalsController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessApproval(Guid orderId, [FromBody] ApprovalRequestDto request)
        {
            // 1. Kiểm tra DTO đầu vào
            if (request.Action != "Approved" && request.Action != "Rejected")
            {
                return BadRequest(new { message = "Hành động chỉ có thể là 'Approved' hoặc 'Rejected'." });
            }

            // 2. Tự động lấy UserId từ trong JWT Token đang đăng nhập
            // (Tùy vào lúc tạo Token bạn đặt tên biến là gì, thường là NameIdentifier hoặc "id")
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Không xác định được danh tính người duyệt từ Token." });
            }

            // 3. Gọi Service để xử lý lưu vào Database
            var isSuccess = await _approvalService.ProcessApprovalAsync(orderId, userId, request);

            if (!isSuccess)
            {
                return NotFound(new { message = "Không tìm thấy hồ sơ này trong hệ thống." });
            }

            return Ok(new
            {
                message = "Xét duyệt thành công!",
                action = request.Action,
                reason = request.Reason
            });
        }
    }
}