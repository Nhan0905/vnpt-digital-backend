using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vnpt.Application.DTOs;
using Vnpt.Domain;

namespace Vnpt.Application.Interfaces
{
    public interface IUserService
    {
        // Hàm đăng ký trả về true/false
        Task<bool> RegisterAsync(RegisterDTO dto);

        // Hàm đăng nhập trả về thông tin gói LoginResponseDTO (Token, Role, FullName)
        Task<LoginResponseDTO?> LoginAsync(LoginDTO dto);

        // Hàm lấy danh sách nhân viên
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<User?> GetProfileByUsernameAsync(string username);
        Task<bool> AdminUpdateUserAsync(Guid id, AdminUpdateUserDto dto);
    }
}