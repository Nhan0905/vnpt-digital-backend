using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;

namespace Vnpt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        // Chỉ tiêm IUserService vào Controller, tuyệt đối không dùng IUserRepository ở đây
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            var isSuccess = await _userService.RegisterAsync(registerDto);
            if (!isSuccess)
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });
            }
            return Ok(new { message = "Đăng ký tài khoản thành công!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            // Nhận kết quả dạng LoginResponseDTO từ Service
            var result = await _userService.LoginAsync(loginDto);

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }

            // Trả về Token, Role và FullName cho phía Frontend
            return Ok(new
            {
                token = result.Token,
                role = result.Role,
                fullName = result.FullName
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { message = "Lấy danh sách người dùng thành công", data = users });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin xác thực!" });
            }

            var user = await _userService.GetProfileByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng!" });
            }

            return Ok(new
            {
                fullName = user.FullName,
                gender = user.Gender,
                cccd = user.CCCD,
                phone = user.Phone,
                email = user.Email,
                province = user.Province,
                district = user.District,
                address = user.Address,
                // Bổ sung thêm 2 dòng này để chắc chắn API đẩy dữ liệu xuống
                customerType = string.IsNullOrEmpty(user.CustomerType) ? "Cá nhân" : user.CustomerType,
                status = string.IsNullOrEmpty(user.Status) ? "Đang hoạt động" : user.Status
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/admin-update")]
        public async Task<IActionResult> AdminUpdateUser(Guid id, [FromBody] AdminUpdateUserDto dto)
        {
            var result = await _userService.AdminUpdateUserAsync(id, dto);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy người dùng cần cập nhật!" });
            }

            return Ok(new { message = "Cập nhật trạng thái và loại khách hàng thành công!" });
        }
    }
}