using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NETCoreApi.Controllers {
    [ApiController]
    [Route("api/")]
    public class PaymentController : ControllerBase {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController (IPaymentRepository paymentRepository) {
            _paymentRepository = paymentRepository;
        }

        // Endpoint: POST /api/payment/add
        [HttpPost("addMethodPayment")]
        public async Task<IActionResult> AddPayment ([FromBody] PaymentDto paymentDto) {
            try {
                // Gọi phương thức từ Repository để thêm Payment
                var result = await _paymentRepository.AddPaymentAsync(paymentDto);

                // Trả về kết quả thành công với mã trạng thái 200
                return Ok(result);
            }
            catch (Exception ex) {
                // Trả về lỗi với mã trạng thái 500 và thông báo lỗi rõ ràng
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

        // Endpoint: GET /api/payment/all

        [HttpGet("allMethodPayment")]
        public async Task<IActionResult> GetAllPayments () {
            try {
                // Gọi phương thức từ Repository để lấy danh sách Payment
                var payments = await _paymentRepository.GetAllPaymentsAsync();

                // Trả về danh sách Payment với mã trạng thái 200
                return Ok(payments);
            }
            catch (Exception ex) {
                // Trả về lỗi với mã trạng thái 500 và thông báo lỗi rõ ràng
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
    }
}
