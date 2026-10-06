using Vnpt.Application.Interfaces;
using Vnpt.Domain.Entities;

namespace Vnpt.Persistence.Repositories
{
    public class ApprovalLogRepository : IApprovalLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ApprovalLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApprovalLog> AddAsync(ApprovalLog log)
        {
            await _context.ApprovalLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }
    }
}