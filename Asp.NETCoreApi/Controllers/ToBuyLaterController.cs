using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.Helper;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NETCoreApi.Controllers {
    public class ToBuyLaterController : Controller {

        private readonly IToBuyLaterRepository _toBuyLaterRepository;

        public ToBuyLaterController (IToBuyLaterRepository toBuyLaterRepository) {
            _toBuyLaterRepository = toBuyLaterRepository;
        }


        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("SaveToBuyLater")]
        public async Task<IActionResult> SaveToBuyLater ([FromBody] int sizeId) {
            // Lấy UserId từ token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }



            // Tạo đối tượng để lưu vào DB
            try {
                // Call the repository method and await the result
                var result = await _toBuyLaterRepository.SaveToBuyLater(sizeId, userId);
                return Ok(new { message = result });
            }
            catch (Exception ex) {
                // Handle exceptions and return a meaningful error response
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }


        }
        [Authorize(Roles = AppRole.Customer)]
        [HttpGet("GetProductsWithSelectedColors")]
        public async Task<IActionResult> GetProductsWithSelectedColors () {
            // Lấy UserId từ token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                // Call the repository method to get products
                var products = await _toBuyLaterRepository.GetProductsWithSelectedColors(userId);
                return Ok(products);
            }
            catch (Exception ex) {
                // Handle any errors gracefully
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }


        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("api/UpdateQuantityInBuyLater")]
        public async Task<IActionResult> UpdateQuantityInBuyLater ([FromBody] ToBuyLaterDto request) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                var result = await _toBuyLaterRepository.UpdateQuantityInBuyLater(request.SizeId, userId, request.Quantity);
                return Ok(new { message = result });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
