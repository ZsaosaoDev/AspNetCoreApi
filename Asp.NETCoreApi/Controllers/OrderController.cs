using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.Helper;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NETCoreApi.Controllers {
    [Route("api/")]
    [ApiController]
    public class OrderController : ControllerBase {
        private readonly IOrderRepository _orderRepository;

        public OrderController (IOrderRepository orderRepository) {
            _orderRepository = orderRepository;
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("Order")]
        public async Task<IActionResult> Order ([FromBody] OrderRequestDto request) {
            // Extract UserId from token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                // Call the repository method
                var result = await _orderRepository.Order(request.SizeIds, userId, request.PaymentId, request.DeliverId);

                if (result.Status == 200) {
                    return Ok(new { message = result.Message });
                }
                return StatusCode(result.Status, new { message = result.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
