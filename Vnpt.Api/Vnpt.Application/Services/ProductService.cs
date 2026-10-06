using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;
using Vnpt.Domain;

namespace Vnpt.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                // Lấy Description từ Database trả về Frontend
                Description = p.Description
            }).ToList();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(Guid id)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                // Lấy Description từ Database trả về Frontend
                Description = p.Description
            };
        }

        public async Task<bool> CreateProductAsync(ProductDTO dto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Name = dto.Name,
                Category = dto.Category,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                // Gán Description từ Frontend gửi lên vào Database
                Description = dto.Description
            };

            await _repository.AddAsync(product);
            return true;
        }

        public async Task<bool> UpdateProductAsync(Guid id, ProductDTO dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            product.Code = dto.Code;
            product.Name = dto.Name;
            product.Category = dto.Category;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl;
            product.IsActive = dto.IsActive;

            // Cập nhật Description mới vào Database
            product.Description = dto.Description;

            await _repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}