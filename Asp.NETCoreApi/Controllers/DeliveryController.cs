using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NETCoreApi.Controllers {
    [ApiController]
    [Route("api")]
    public class DeliveryController : ControllerBase {
        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveryController (IDeliveryRepository deliveryRepository) {
            _deliveryRepository = deliveryRepository;
        }

        // Thêm Delivery
        [HttpPost("addDelivery")]
        public async Task<IActionResult> AddDelivery ([FromBody] DeliveryDto deliveryDto) {
            try {
                // Gọi phương thức AddDeliveryAsync từ repository
                var result = await _deliveryRepository.AddDeliveryAsync(deliveryDto);
                return Ok(result); // Trả về kết quả thành công
            }
            catch (Exception ex) {
                // Bắt ngoại lệ nếu có lỗi
                return StatusCode(500, new { Error = $"An error occurred while adding delivery: {ex.Message}" });
            }
        }

        // Lấy tất cả Delivery
        [HttpGet("allDelivery")]
        public async Task<IActionResult> GetAllDeliveries () {
            try {
                // Gọi phương thức GetAllDeliveryAsync từ repository
                var deliveries = await _deliveryRepository.GetAllDeliveryAsync();
                if (deliveries == null || deliveries.Count == 0) {
                    return NotFound(new { Message = "No deliveries found." }); // Nếu không có dữ liệu, trả về NotFound
                }

                return Ok(deliveries); // Trả về danh sách deliveries
            }
            catch (Exception ex) {
                // Bắt ngoại lệ nếu có lỗi
                return StatusCode(500, new { Error = $"An error occurred while retrieving deliveries: {ex.Message}" });
            }
        }
    }
}
