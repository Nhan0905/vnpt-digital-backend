using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Domain.Entities;

namespace Vnpt.Application.Interfaces
{
    public interface IOrderDocumentRepository
    {
        Task<OrderDocument> AddAsync(OrderDocument document);
        // Sau này có thể thêm hàm lấy danh sách tài liệu của 1 hồ sơ ở đây
    }
}