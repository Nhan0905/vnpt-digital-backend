using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Application.DTOs;

namespace Vnpt.Application.Interfaces
{
    public interface IExcelService
    {
        byte[] GenerateDashboardReport(DashboardStatsDto stats);
    }
}