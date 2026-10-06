using System;
using System.Collections.Generic;
using System.Text;
using Vnpt.Application.DTOs;

namespace Vnpt.Application.Interfaces
{
    public interface ICustomerService
    {
        // Khai báo 2 hành động: Lấy danh sách và Thêm mới
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customerDto);
        Task<bool> DeleteCustomerAsync(Guid id);
        Task<bool> UpdateCustomerAsync(Guid id, CustomerDTO dto);
    }
}