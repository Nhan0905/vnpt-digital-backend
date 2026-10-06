using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Domain.Entities;

namespace Vnpt.Application.Interfaces
{
    public interface IOrderDocumentService
    {
        Task<OrderDocument> AddDocumentRecordAsync(Guid orderId, string fileName, string filePath, string fileType, long fileSize);
    }
}