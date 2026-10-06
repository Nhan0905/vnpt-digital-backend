using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Application.DTOs;
using Vnpt.Domain;
using Vnpt.Domain.Entities;

namespace Vnpt.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO?> GetOrderByIdAsync(Guid id);

        // Cập nhật dòng này: Truyền thêm customerId và trả về OrderDTO
        Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto, Guid customerId);

        Task<bool> ApproveOrderAsync(Guid id);
        Task<bool> RejectOrderAsync(Guid id, string reason);
        Task<PagedResult<Order>> GetPagedOrdersAsync(int pageNumber, int pageSize, string? searchTerm, string? status);
        Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(Guid customerId);
        Task<bool> ResubmitOrderAsync(Guid id, CreateOrderDTO dto);
        Task<IEnumerable<ApprovalLog>> GetOrderLogsAsync(Guid orderId);
        Task<bool> AddDocumentAsync(Guid orderId, string fileName, string filePath, string fileType, long fileSize);

        Task<IEnumerable<OrderDocument>> GetDocumentsByOrderIdAsync(Guid orderId);
        Task<bool> ApproveOrderAsync(Guid id, Guid adminId);
        Task<bool> RejectOrderAsync(Guid id, string reason, Guid adminId);
    }
}
