using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;
using Vnpt.Domain;

namespace Vnpt.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        // Hàm xử lý RegisterAsync trong UserService
        public async Task<bool> RegisterAsync(RegisterDTO registerDto)
        {
            // Kiểm tra tên đăng nhập đã tồn tại chưa
            var exists = await _repository.CheckUsernameExistsAsync(registerDto.Username);
            if (exists) return false;

            // Khởi tạo User với đầy đủ thông tin từ DTO truyền xuống
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = registerDto.Username,
                // Dùng BCrypt hoặc mã hóa tương ứng bạn đang dùng
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                Role = registerDto.Role,

                // MAP DỮ LIỆU TỪ FORM ĐĂNG KÝ VÀO DATABASE
                Gender = registerDto.Gender,
                CCCD = registerDto.CCCD,
                Phone = registerDto.Phone,
                Email = registerDto.Email,
                Province = registerDto.Province,
                District = registerDto.District,
                Address = registerDto.Address,

                // CÁC THÔNG TIN SET MẶC ĐỊNH CHO TÀI KHOẢN MỚI
                CustomerType = "Cá nhân",
                Status = "Đang hoạt động",
                JoinDate = $"{DateTime.Now.Year} - {DateTime.Now.Year + 4}", // Sinh tự động niên khóa (VD: 2026 - 2030)

                IsActive = true,
                IsDeleted = false
            };

            // Gọi đúng hàm AddAsync
            await _repository.AddAsync(newUser);
            return true;
        }

        // Đã đồng bộ hoàn toàn với IUserService
        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var user = await _repository.GetByUsernameAsync(dto.Username);
            if (user == null) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid) return null;

            // TẠO JWT TOKEN
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Trả về đối tượng LoginResponseDTO chứa Token, Role và FullName
            return new LoginResponseDTO
            {
                Token = tokenString,
                Role = user.Role ?? "Staff",
                FullName = user.FullName ?? user.Username
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();

            return users.Select(u => new UserDTO
            {
                Id = u.Id.ToString(),
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                IsActive = u.IsActive
            }).ToList();
        }

        public async Task<User?> GetProfileByUsernameAsync(string username)
        {
            return await _repository.GetByUsernameAsync(username);
        }

        public async Task<bool> AdminUpdateUserAsync(Guid id, AdminUpdateUserDto dto)
        {
            // Tìm user theo ID
            var user = await _repository.GetByIdAsync(id); // (Đảm bảo Repository của bạn có hàm GetByIdAsync)
            if (user == null) return false;

            // Cập nhật các trường quản lý
            user.CustomerType = !string.IsNullOrEmpty(dto.CustomerType) ? dto.CustomerType : user.CustomerType;
            user.Status = !string.IsNullOrEmpty(dto.Status) ? dto.Status : user.Status;
            user.Role = !string.IsNullOrEmpty(dto.Role) ? dto.Role : user.Role;

            await _repository.UpdateAsync(user); // (Hoặc _repository.SaveChangesAsync())
            return true;
        }
    }
}