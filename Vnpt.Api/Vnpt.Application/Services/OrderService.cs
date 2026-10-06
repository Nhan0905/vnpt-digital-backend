using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;
using Vnpt.Domain;
using Vnpt.Domain.Entities;

namespace Vnpt.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                CustomerId = o.CustomerId,
                ProductId = o.ProductId,
                Status = o.Status,
                RejectReason = o.RejectReason,
                CreatedAt = o.CreatedAt,

                // --- ĐÃ BỔ SUNG 4 TRƯỜNG NÀY ĐỂ FRONTEND HIỂN THỊ ---
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                Address = o.Address,
                Note = o.Note
            });
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            return new OrderDTO
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Status = order.Status,
                RejectReason = order.RejectReason,
                CreatedAt = order.CreatedAt,

                // --- ĐÃ BỔ SUNG 4 TRƯỜNG NÀY ---
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Address = order.Address,
                Note = order.Note
            };
        }

        public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto, Guid customerId)
        {
            int countToday = await _orderRepository.CountOrdersTodayAsync();

            string datePart = DateTime.Now.ToString("yyMMdd");
            string sequence = (countToday + 1).ToString("D4");
            string orderCode = $"HS{datePart}-{sequence}";

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderCode = orderCode,
                CustomerId = customerId,
                ProductId = dto.ProductId,
                CustomerName = dto.CustomerName ?? string.Empty,
                Phone = dto.Phone ?? string.Empty,
                Address = dto.Address ?? string.Empty,
                Note = dto.Note ?? string.Empty,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var savedOrder = await _orderRepository.AddAsync(order);

            // --- TỰ ĐỘNG GHI AUDIT LOG: TẠO MỚI ---
            await _orderRepository.AddApprovalLogAsync(new ApprovalLog
            {
                Id = Guid.NewGuid(),
                OrderId = savedOrder.Id,
                UserId = customerId,
                Action = "Created",
                Reason = "Khách hàng tạo mới hồ sơ đăng ký dịch vụ",
                CreatedAt = DateTime.UtcNow
            });

            return new OrderDTO
            {
                Id = savedOrder.Id,
                OrderCode = savedOrder.OrderCode,
                CustomerId = savedOrder.CustomerId,
                ProductId = savedOrder.ProductId,
                Status = savedOrder.Status,
                CreatedAt = savedOrder.CreatedAt,
                CustomerName = savedOrder.CustomerName,
                Phone = savedOrder.Phone,
                Address = savedOrder.Address,
                Note = savedOrder.Note
            };
        }

        public async Task<bool> ApproveOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null || order.Status != "Pending") return false;

            order.Status = "Approved";
            var success = await _orderRepository.UpdateAsync(order);

            if (success)
            {
                // --- TỰ ĐỘNG GHI AUDIT LOG: PHÊ DUYỆT ---
                await _orderRepository.AddApprovalLogAsync(new ApprovalLog
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    UserId = Guid.Empty, // Có thể thay bằng Admin ID từ token nếu cần
                    Action = "Approved",
                    Reason = "Hồ sơ hợp lệ, đã được phê duyệt",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return success;
        }

        public async Task<bool> RejectOrderAsync(Guid id, string reason)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null || order.Status != "Pending") return false;

            order.Status = "Rejected";
            order.RejectReason = reason;
            var success = await _orderRepository.UpdateAsync(order);

            if (success)
            {
                // --- TỰ ĐỘNG GHI AUDIT LOG: TỪ CHỐI ---
                await _orderRepository.AddApprovalLogAsync(new ApprovalLog
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    UserId = Guid.Empty,
                    Action = "Rejected",
                    Reason = reason,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return success;
        }

        public async Task<PagedResult<Order>> GetPagedOrdersAsync(int pageNumber, int pageSize, string? searchTerm, string? status)
        {
            var (orders, totalCount) = await _orderRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, status);

            return new PagedResult<Order>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                CustomerId = o.CustomerId,
                ProductId = o.ProductId,
                Status = o.Status,
                RejectReason = o.RejectReason,
                CreatedAt = o.CreatedAt,

                // --- ĐÃ BỔ SUNG 4 TRƯỜNG NÀY ---
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                Address = o.Address,
                Note = o.Note
            });
        }

        public async Task<bool> ResubmitOrderAsync(Guid id, CreateOrderDTO dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null || order.Status != "Rejected") return false;

            order.CustomerName = dto.CustomerName!;
            order.Phone = dto.Phone!;
            order.Address = dto.Address!;
            order.Note = dto.Note!;
            order.Status = "Pending";
            order.RejectReason = null;

            var success = await _orderRepository.UpdateAsync(order);

            if (success)
            {
                // --- TỰ ĐỘNG GHI AUDIT LOG: CẬP NHẬT LẠI ---
                await _orderRepository.AddApprovalLogAsync(new ApprovalLog
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    UserId = order.CustomerId,
                    Action = "Resubmitted",
                    Reason = "Khách hàng chỉnh sửa và gửi lại hồ sơ",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return success;
        }

        // --- HÀM LẤY LỊCH SỬ LOG CHO TIMELINE ---
        public async Task<IEnumerable<ApprovalLog>> GetOrderLogsAsync(Guid orderId)
        {
            return await _orderRepository.GetApprovalLogsByOrderIdAsync(orderId);
        }

        // --- HÀM LƯU TÀI LIỆU ĐÍNH KÈM ---
        public async Task<bool> AddDocumentAsync(Guid orderId, string fileName, string filePath, string fileType, long fileSize)
        {
            var document = new OrderDocument
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                FileName = fileName,
                FilePath = filePath,
                FileType = fileType,
                FileSize = fileSize,
                Version = 1,
                CreatedAt = DateTime.UtcNow
            };
            return await _orderRepository.AddDocumentAsync(document);
        }

        public async Task<IEnumerable<OrderDocument>> GetDocumentsByOrderIdAsync(Guid orderId)
        {
            return await _orderRepository.GetDocumentsByOrderIdAsync(orderId);
        }

        public async Task<bool> ApproveOrderAsync(Guid id, Guid adminId)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null || order.Status != "Pending") return false;

            order.Status = "Approved";
            var success = await _orderRepository.UpdateAsync(order);

            if (success)
            {
                await _orderRepository.AddApprovalLogAsync(new ApprovalLog
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    UserId = adminId, // <-- Ghi nhận đích danh ID của Admin
                    Action = "Approved",
                    Reason = "Hồ sơ hợp lệ, đã được phê duyệt",
                    CreatedAt = DateTime.UtcNow
                });
            }
            return success;
        }

        public async Task<bool> RejectOrderAsync(Guid id, string reason, Guid adminId)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null || order.Status != "Pending") return false;

            order.Status = "Rejected";
            order.RejectReason = reason;
            var success = await _orderRepository.UpdateAsync(order);

            if (success)
            {
                await _orderRepository.AddApprovalLogAsync(new ApprovalLog
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    UserId = adminId, // <-- Ghi nhận đích danh ID của Admin
                    Action = "Rejected",
                    Reason = reason,
                    CreatedAt = DateTime.UtcNow
                });
            }
            return success;
        }
    }
}