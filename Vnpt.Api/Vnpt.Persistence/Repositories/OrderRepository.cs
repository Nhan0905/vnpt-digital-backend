using Microsoft.EntityFrameworkCore;
using Vnpt.Application.Interfaces;
using Vnpt.Domain;
using Vnpt.Domain.Entities;

namespace Vnpt.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Đếm số đơn hàng được tạo trong ngày hôm nay
        public async Task<int> CountOrdersTodayAsync()
        {
            var today = DateTime.Today;
            return await _context.Orders
                .Where(o => o.CreatedAt.Date == today && !o.IsDeleted)
                .CountAsync();
        }

        // Thêm đơn hàng mới (ĐÃ FIX LỖI KHÓA NGOẠI CUSTOMER)
        public async Task<Order> AddAsync(Order order)
        {
            // Kiểm tra xem bảng Customers đã có khách hàng này chưa, nếu chưa thì tự động thêm vào
            var customerInDb = await _context.Customers.FindAsync(order.CustomerId);
            if (customerInDb == null)
            {
                var newCustomer = new Customer
                {
                    Id = order.CustomerId,
                    FullName = order.CustomerName,
                    Phone = order.Phone,
                    Address = order.Address
                };
                await _context.Customers.AddAsync(newCustomer);
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .Where(o => !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        // Hoàn thiện luôn phần Phân trang và Tìm kiếm cho Admin
        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, string? status)
        {
            var query = _context.Orders
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .Where(o => !o.IsDeleted)
                .AsQueryable();

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            // Tìm kiếm theo mã hồ sơ, tên hoặc số điện thoại
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(o => o.OrderCode.Contains(searchTerm) ||
                                         o.CustomerName.Contains(searchTerm) ||
                                         o.Phone.Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Where(o => o.CustomerId == customerId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // --- THỰC THI GHI VÀ LẤY LOG ---
        public async Task AddApprovalLogAsync(ApprovalLog log)
        {
            await _context.ApprovalLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApprovalLog>> GetApprovalLogsByOrderIdAsync(Guid orderId)
        {
            return await _context.ApprovalLogs
                .Where(l => l.OrderId == orderId && !l.IsDeleted)
                .OrderBy(l => l.CreatedAt)
                .ToListAsync();
        }

        // --- THỰC THI LƯU TÀI LIỆU ---
        public async Task<bool> AddDocumentAsync(OrderDocument document)
        {
            await _context.OrderDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderDocument>> GetDocumentsByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderDocuments
                .Where(d => d.OrderId == orderId && !d.IsDeleted)
                .ToListAsync();
        }
    }
}