using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.Helper;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NETCoreApi.Controllers {
    [ApiController]
    [Route("api/")]
    public class ToBuyLaterController : ControllerBase {
        private readonly IToBuyLaterRepository _toBuyLaterRepository;

        public ToBuyLaterController (IToBuyLaterRepository toBuyLaterRepository) {
            _toBuyLaterRepository = toBuyLaterRepository;
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("SaveToBuyLater")]
        public async Task<IActionResult> SaveToBuyLater ([FromBody] int sizeId) {
            // Extract UserId from token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                // Call the repository method
                var result = await _toBuyLaterRepository.SaveToBuyLater(sizeId, userId);

                if (result.Status == 200) {
                    return Ok(new { message = result.Message });
                }
                return StatusCode(result.Status, new { message = result.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpGet("GetProductsWithSelectedColors")]
        public async Task<IActionResult> GetProductsWithSelectedColors () {
            // Extract UserId from token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                // Call the repository method
                var products = await _toBuyLaterRepository.GetProductsWithSelectedColors(userId);
                return Ok(new { products });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("UpdateQuantityInBuyLater")]
        public async Task<IActionResult> UpdateQuantityInBuyLater ([FromBody] ToBuyLaterDto request) {
            // Extract UserId from token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                // Call the repository method
                var result = await _toBuyLaterRepository.UpdateQuantityInBuyLater(request.SizeId, userId, request.Quantity);

                if (result.Status == 200) {
                    return Ok(new { message = result.Message });
                }
                return StatusCode(result.Status, new { message = result.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("UpdateAddQuantityInBuyLater")]
        public async Task<IActionResult> UpdateAddQuantityInBuyLater ([FromBody] ToBuyLaterDto request) {
            // Extract UserId from token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                // Call the repository method
                var result = await _toBuyLaterRepository.UpdateAddQuantityInBuyLater(request.SizeId, userId, request.Quantity);

                if (result.Status == 200) {
                    return Ok(new { message = result.Message });
                }
                return StatusCode(result.Status, new { message = result.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("RemoveFromBuyLater")]
        public async Task<IActionResult> RemoveProductFromBuyLater ([FromBody] int sizeId) {
            // Extract UserId from token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { message = "User ID not found" });
            }

            try {
                // Call the repository method
                var result = await _toBuyLaterRepository.RemoveFromBuyLater(sizeId, userId);

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
