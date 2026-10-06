using System;
using System.Collections.Generic;
using System.Text;

using Vnpt.Application.Interfaces;
using Vnpt.Domain.Entities;

namespace Vnpt.Persistence.Repositories
{
    public class OrderDocumentRepository : IOrderDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDocument> AddAsync(OrderDocument document)
        {
            await _context.OrderDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
            return document;
        }
    }
}