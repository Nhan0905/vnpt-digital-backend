using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;
using Vnpt.Domain.Entities;

namespace Vnpt.Application.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IApprovalLogRepository _repository;

        public ApprovalService(IApprovalLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ProcessApprovalAsync(Guid orderId, Guid userId, ApprovalRequestDto request)
        {
            var log = new ApprovalLog
            {
                OrderId = orderId,
                UserId = userId,
                Action = request.Action,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(log);
            return true;
        }
    }
}