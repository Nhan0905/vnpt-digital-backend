using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vnpt.Domain;
using Vnpt.Persistence;

namespace Vnpt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API lấy danh sách điểm giao dịch (Khách hàng gọi để tra cứu)
        [HttpGet]
        public async Task<IActionResult> GetStores([FromQuery] string? province, [FromQuery] string? district)
        {
            var query = _context.Stores.AsQueryable();

            // Lọc theo Tỉnh/TP (so sánh không phân biệt hoa thường)
            if (!string.IsNullOrEmpty(province))
            {
                query = query.Where(s => s.Province.ToLower() == province.ToLower() || s.Address.ToLower().Contains(province));
            }

            // Lọc theo Quận/Huyện nếu có nhập và không phải là "all"
            if (!string.IsNullOrEmpty(district) && district != "all")
            {
                query = query.Where(s => s.District.ToLower().Contains(district.ToLower()) || s.Address.ToLower().Contains(district));
            }

            var stores = await query.ToListAsync();
            return Ok(new { data = stores });
        }

        // --- API MỚI: DÀNH CHO ADMIN THÊM ĐIỂM GIAO DỊCH ---
        [HttpPost]
        [Authorize] // Yêu cầu đăng nhập tài khoản Admin
        public async Task<IActionResult> CreateStore([FromBody] Store store)
        {
            if (store == null)
                return BadRequest(new { message = "Dữ liệu điểm giao dịch không hợp lệ!" });

            store.Id = Guid.NewGuid();
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm điểm giao dịch thành công!", data = store });
        }

        // --- API XÓA ĐIỂM GIAO DỊCH ---
        [HttpDelete("{id}")]
        [Authorize] // Yêu cầu đăng nhập quyền Admin
        public async Task<IActionResult> DeleteStore(Guid id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound(new { message = "Không tìm thấy điểm giao dịch cần xóa!" });
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa điểm giao dịch thành công!" });
        }
    }
}