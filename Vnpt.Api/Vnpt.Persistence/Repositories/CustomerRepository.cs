using Microsoft.EntityFrameworkCore;
using Vnpt.Application.Interfaces;
using Vnpt.Domain;

namespace Vnpt.Persistence.Repositories
{
    // Class này ký hợp đồng ICustomerRepository
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        // Tiêm DbContext vào để làm việc với SQL Server
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            // Truy vấn lấy danh sách từ Database
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            // Thêm vào Database và lưu lại
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer; // Trả về entity đã có ID
        }

        public async Task<bool> CheckCCCDExistsAsync(string identityNo)
        {
            // AnyAsync sẽ trả về true nếu tìm thấy ít nhất 1 dòng có CCCD trùng khớp
            return await _context.Customers.AnyAsync(c => c.IdentityNo == identityNo);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // 1. Tìm khách hàng theo ID
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return false; // Không tìm thấy
            }

            // 2. XÓA MỀM: Không dùng lệnh _context.Customers.Remove()
            // Mà chỉ chuyển trạng thái IsDeleted thành true
            customer.IsDeleted = true;

            // 3. Lưu lại thay đổi
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}