using Microsoft.AspNetCore.Mvc;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;

namespace Vnpt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(new { data = products });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO dto)
        {
            await _productService.CreateProductAsync(dto);
            return Ok(new { message = "Thêm dịch vụ thành công!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductDTO dto)
        {
            var success = await _productService.UpdateProductAsync(id, dto);
            if (!success) return NotFound(new { message = "Không tìm thấy dịch vụ!" });
            return Ok(new { message = "Cập nhật thành công!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok(new { message = "Xóa dịch vụ thành công!" });
        }
    }
}