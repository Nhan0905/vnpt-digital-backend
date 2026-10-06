using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Domain;
using Vnpt.Domain.Entities;

namespace Vnpt.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order> AddAsync(Order order);
        Task<bool> UpdateAsync(Order order);

        // Cực kỳ quan trọng: Hàm đếm số hồ sơ trong ngày để tạo mã tự động (VD: 0001, 0002)
        Task<int> CountOrdersTodayAsync();
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, string? status);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
        Task AddApprovalLogAsync(ApprovalLog log);
        Task<IEnumerable<ApprovalLog>> GetApprovalLogsByOrderIdAsync(Guid orderId);
        Task<bool> AddDocumentAsync(OrderDocument document);
        Task<IEnumerable<OrderDocument>> GetDocumentsByOrderIdAsync(Guid orderId);
    }
}