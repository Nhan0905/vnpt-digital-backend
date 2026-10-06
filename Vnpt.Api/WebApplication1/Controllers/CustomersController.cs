using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vnpt.Application.DTOs;
using Vnpt.Application.Interfaces;

namespace Vnpt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        // Controller chỉ nhận Interface, không quan tâm bên trong làm gì
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> PostCustomer(CustomerDTO customerDto)
        {
            try
            {
                var result = await _customerService.CreateCustomerAsync(customerDto);
                return CreatedAtAction(nameof(GetCustomers), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                // Báo lỗi 400 Bad Request kèm theo câu thông báo từ Service
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var isDeleted = await _customerService.DeleteCustomerAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng để xóa!" });
            }

            return Ok(new { message = "Đã xóa khách hàng thành công!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(Guid id, CustomerDTO customerDto)
        {
            var isSuccess = await _customerService.UpdateCustomerAsync(id, customerDto);

            if (!isSuccess)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng để cập nhật!" });
            }

            return Ok(new { message = "Đã cập nhật thông tin khách hàng thành công!" });
        }
    }
}