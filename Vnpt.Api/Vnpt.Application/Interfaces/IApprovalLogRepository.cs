using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Domain.Entities;

namespace Vnpt.Application.Interfaces
{
    public interface IApprovalLogRepository
    {
        Task<ApprovalLog> AddAsync(ApprovalLog log);
    }
}