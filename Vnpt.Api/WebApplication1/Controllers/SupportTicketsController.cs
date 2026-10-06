using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vnpt.Domain;
using Vnpt.Persistence;

namespace Vnpt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportTicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SupportTicketsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. Khách hàng tạo ticket hỗ trợ / báo hỏng mạng
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTicket(
            [FromForm] string title,
            [FromForm] string content,
            [FromForm] string phone,
            [FromForm] string customerName,
            IFormFile? issueImage)
        {
            string? uniqueFileName = null;
            if (issueImage != null && issueImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "tickets");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + issueImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await issueImage.CopyToAsync(fileStream);
                }
                uniqueFileName = $"/uploads/tickets/{uniqueFileName}";
            }

            var ticket = new SupportTicket
            {
                CustomerName = customerName,
                Phone = phone,
                Title = title,
                Content = content,
                ImagePath = uniqueFileName,
                Status = "Chờ tiếp nhận",
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Gửi yêu cầu hỗ trợ thành công! Nhân viên sẽ liên hệ sớm.", data = ticket });
        }

        // 2. Admin/Staff lấy toàn bộ danh sách ticket
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.SupportTickets.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return Ok(new { data = tickets });
        }

        // 3. Khách hàng lấy danh sách ticket của chính họ
        [HttpGet("my-tickets")]
        [Authorize]
        public async Task<IActionResult> GetMyTickets()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var tickets = await _context.SupportTickets
                .Where(t => t.Phone == user.Phone || t.CustomerName == user.FullName)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(new { data = tickets });
        }

        // 4. Admin/Staff phân công kỹ thuật & cập nhật phản hồi
        [HttpPut("{id}/process")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ProcessTicket(Guid id, [FromBody] ProcessTicketDto dto)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound(new { message = "Không tìm thấy yêu cầu hỗ trợ!" });

            ticket.Status = dto.Status;
            ticket.AssignedTechnician = dto.AssignedTechnician;
            ticket.ResponseNote = dto.ResponseNote;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái ticket thành công!" });
        }
    }

    public class ProcessTicketDto
    {
        public string Status { get; set; } = string.Empty;
        public string AssignedTechnician { get; set; } = string.Empty;
        public string ResponseNote { get; set; } = string.Empty;
    }
}