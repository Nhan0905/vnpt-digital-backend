using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;
using Vnpt.Domain;

namespace Vnpt.Application.Services
{
    // Ký hợp đồng ICustomerService
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        // Tiêm "Quản lý kho" vào để Service sử dụng
        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            // 1. Nhờ kho lấy ra danh sách Entity gốc
            var customers = await _repository.GetAllAsync();

            // 2. Chuyển đổi Entity gốc (Công thức mật) sang DTO (Thực đơn) để trả về
            var customerDtos = customers.Select(c => new CustomerDTO
            {
                Id = c.Id,
                FullName = c.FullName,
                IdentityNo = c.IdentityNo,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address
            });

            return customerDtos;
        }

        public async Task<CustomerDTO> CreateCustomerAsync(CustomerDTO dto)
        {
            // 1. Kiểm tra quy tắc nghiệp vụ: Trùng CCCD
            bool isExists = await _repository.CheckCCCDExistsAsync(dto.IdentityNo);
            if (isExists)
            {
                // Ném ra một lỗi để chặn quá trình lưu lại
                throw new Exception($"Khách hàng với số CCCD {dto.IdentityNo} đã tồn tại trong hệ thống!");
            }

            // 2. Chuyển đổi DTO (Thông tin khách nhập) thành Entity (Dữ liệu thô để cất kho)
            var customer = new Customer
            {
                Id = Guid.NewGuid(), // Tự tạo ID mới
                FullName = dto.FullName,
                IdentityNo = dto.IdentityNo,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                IsDeleted = false
            };

            // 3. Nhờ kho cất đi
            await _repository.AddAsync(customer);

            // 4. Trả về DTO kèm theo ID vừa tạo
            dto.Id = customer.Id;
            return dto;
        }
        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            // Gọi xuống kho để thực hiện xóa mềm
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, CustomerDTO dto)
        {
            // 1. Nhờ kho tìm xem khách hàng có tồn tại không
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null)
            {
                return false; // Không tìm thấy
            }

            // 2. Cập nhật thông tin mới từ DTO (Thực đơn) sang Entity (Công thức)
            customer.FullName = dto.FullName;
            customer.IdentityNo = dto.IdentityNo;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;

            // 3. Nhờ kho lưu lại
            return await _repository.UpdateAsync(customer);
        }
    }
}