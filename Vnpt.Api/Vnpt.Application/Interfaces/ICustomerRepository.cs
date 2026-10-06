using System;
using System.Collections.Generic;
using System.Text;
using Vnpt.Domain;

namespace Vnpt.Application.Interfaces
{
    public interface ICustomerRepository
    {
        // Giao tiếp trực tiếp với Entity gốc
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> AddAsync(Customer customer);
        // Thêm hàm kiểm tra tồn tại CCCD
        Task<bool> CheckCCCDExistsAsync(string identityNo);
        // Cập nhật trạng thái IsDeleted = true
        Task<bool> DeleteAsync(Guid id);

        // Tìm khách hàng theo ID
        Task<Customer?> GetByIdAsync(Guid id);
        // Cập nhật thông tin
        Task<bool> UpdateAsync(Customer customer);
    }
}