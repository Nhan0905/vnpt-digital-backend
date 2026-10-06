using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vnpt.Domain;

namespace Vnpt.Application.Interfaces
{
    public interface IUserRepository
    {
        // Tìm nhân viên theo tên đăng nhập (để kiểm tra lúc login)
        Task<User?> GetByUsernameAsync(string username);

        // Thêm nhân viên/người dùng mới vào DB
        Task<User> AddAsync(User user);

        Task<IEnumerable<User>> GetAllAsync();

        Task<bool> CheckUsernameExistsAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task UpdateAsync(User user);
    }
}