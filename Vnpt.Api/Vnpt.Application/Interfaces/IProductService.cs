using System;
using System.Collections.Generic;
using System.Text;
using Vnpt.Application.DTOs;

namespace Vnpt.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(Guid id);
        Task<bool> CreateProductAsync(ProductDTO dto);
        Task<bool> UpdateProductAsync(Guid id, ProductDTO dto);
        Task<bool> DeleteProductAsync(Guid id);
    }
}