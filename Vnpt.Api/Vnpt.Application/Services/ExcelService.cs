using System;
using System.Collections.Generic;
using System.Text;

using ClosedXML.Excel;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;

namespace Vnpt.Application.Services
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerateDashboardReport(DashboardStatsDto stats)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Báo cáo thống kê");

                // 1. Tạo tiêu đề
                worksheet.Cell(1, 1).Value = "BÁO CÁO THỐNG KÊ HỒ SƠ VNPT";
                worksheet.Range("A1:B1").Merge().Style.Font.SetBold().Font.FontSize = 14;

                // 2. Tạo Header cho bảng
                worksheet.Cell(3, 1).Value = "Chỉ tiêu";
                worksheet.Cell(3, 2).Value = "Số lượng";
                worksheet.Range("A3:B3").Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);

                // 3. Đổ dữ liệu vào
                worksheet.Cell(4, 1).Value = "Tổng số hồ sơ";
                worksheet.Cell(4, 2).Value = stats.TotalOrders;

                worksheet.Cell(5, 1).Value = "Hồ sơ đã duyệt";
                worksheet.Cell(5, 2).Value = stats.ApprovedOrders;

                worksheet.Cell(6, 1).Value = "Hồ sơ bị từ chối";
                worksheet.Cell(6, 2).Value = stats.RejectedOrders;

                worksheet.Cell(7, 1).Value = "Hồ sơ đang chờ";
                worksheet.Cell(7, 2).Value = stats.PendingOrders;

                // Căn chỉnh cột cho đẹp
                worksheet.Columns().AdjustToContents();

                // 4. Lưu ra mảng byte để gửi về cho người dùng tải
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}