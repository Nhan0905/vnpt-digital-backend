using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Application.Interfaces;
using Vnpt.Domain.Entities;

namespace Vnpt.Application.Services
{
    public class OrderDocumentService : IOrderDocumentService
    {
        private readonly IOrderDocumentRepository _repository;

        public OrderDocumentService(IOrderDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderDocument> AddDocumentRecordAsync(Guid orderId, string fileName, string filePath, string fileType, long fileSize)
        {
            var document = new OrderDocument
            {
                OrderId = orderId,
                FileName = fileName,
                FilePath = filePath,
                FileType = fileType,
                FileSize = fileSize,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.AddAsync(document);
        }
    }
}