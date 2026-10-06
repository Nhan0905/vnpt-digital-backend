using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Application.DTOs;

namespace Vnpt.Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardStatsDto> GetStatisticsAsync();
    }
}