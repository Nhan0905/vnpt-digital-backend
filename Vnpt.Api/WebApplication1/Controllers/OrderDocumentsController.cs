using Microsoft.AspNetCore.Mvc;
using Vnpt.Api.DTOs;
using Vnpt.Application.Interfaces;

namespace Vnpt.Api.Controllers
{
    [Route("api/orders/{orderId}/documents")] // Định tuyến chuẩn RESTful API
    [ApiController]
    public class OrderDocumentsController : ControllerBase
    {
        private readonly IOrderDocumentService _documentService;
        private readonly IWebHostEnvironment _env;

        public OrderDocumentsController(IOrderDocumentService documentService, IWebHostEnvironment env)
        {
            _documentService = documentService;
            _env = env; // Dùng để lấy đường dẫn vật lý của máy chủ
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(Guid orderId, [FromForm] UploadDocumentDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn một file để tải lên." });

            // 1. Kiểm tra dung lượng (Giới hạn 5MB)
            const long maxFileSize = 5 * 1024 * 1024;
            if (dto.File.Length > maxFileSize)
                return BadRequest(new { message = "Dung lượng file không được vượt quá 5MB." });

            // 2. Kiểm tra định dạng (.jpg, .jpeg, .png, .pdf) theo yêu cầu đề cương
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var fileExtension = Path.GetExtension(dto.File.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "Chỉ chấp nhận file định dạng .jpg, .jpeg, .png, .pdf" });

            // 3. Tạo thư mục wwwroot/uploads nếu chưa có
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 4. Đổi tên file để tránh trùng lặp (Thêm mã Guid vào trước tên gốc)
            var uniqueFileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 5. Lưu file vật lý vào ổ cứng
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // 6. Lưu thông tin file vào Database thông qua Service
            // Đường dẫn lưu vào DB là đường dẫn tương đối để sau này hiển thị trên Web/App
            var dbPath = $"/uploads/{uniqueFileName}";
            var documentRecord = await _documentService.AddDocumentRecordAsync(orderId, dto.File.FileName, dbPath, fileExtension, dto.File.Length);

            return Ok(new { message = "Tải tài liệu lên thành công!", data = documentRecord });
        }
    }
}