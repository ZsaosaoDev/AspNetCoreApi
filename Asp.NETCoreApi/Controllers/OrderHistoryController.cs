using Asp.NETCoreApi.Helper;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NETCoreApi.Controllers {
    [Route("api/")]
    [ApiController]
    public class OrderHistoryController : Controller {

        private readonly IOrderHistory _orderHistory;

        public OrderHistoryController (IOrderHistory orderHistory) {
            _orderHistory = orderHistory;
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpGet("GetOrdersByUserId")]
        public async Task<IActionResult> GetOrdersByUserId () {
            try {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId)) {
                    return Unauthorized(new { Message = "User is not authenticated or userId is missing." });
                }

                var orders = await _orderHistory.GetOrdersByUserId(userId);
                return Ok(orders);
            }
            catch (Exception ex) {
                // Log exception here (e.g., using ILogger)
                return StatusCode(500, new { Message = "An error occurred while retrieving the orders.", Error = ex.Message });
            }
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpGet("GetProductsByOrderIdAsync")]
        public async Task<IActionResult> GetProductsByOrderIdAsync (int orderId) {
            try {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId)) {
                    return Unauthorized(new { Message = "User is not authenticated or userId is missing." });
                }

                var products = await _orderHistory.GetProductsByOrderIdAsync(orderId, userId);

                if (products == null || !products.Any()) {
                    return NotFound(new { Message = $"No products found for orderId: {orderId}" });
                }

                return Ok(products);
            }
            catch (UnauthorizedAccessException ex) {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex) {
                // Log exception here (e.g., using ILogger)
                return StatusCode(500, new { Message = "An error occurred while retrieving the products.", Error = ex.Message });
            }
        }
    }
}
