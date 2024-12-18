using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NETCoreApi.Controllers {
    public class ProductController : Controller {

        private readonly IProductRepository _productRepository;

        public ProductController (IProductRepository productRepository) {
            _productRepository = productRepository;
        }
        [HttpPost]
        [Route("api/products")]
        public async Task<IActionResult> AddProduct ([FromBody] ProductDto productDto) {
            if (productDto == null) {
                return BadRequest(new { Error = "ProductDto cannot be null." });
            }

            // Call the service method
            try {
                var result = await _productRepository.AddProduct(productDto);
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("api/getProductById")]
        public async Task<ActionResult<ProductDto>> GetProductById (int id) {
            var productDto = await _productRepository.GetProductDtoById(id);  // Call your service method

            if (productDto == null) {
                return NotFound();
            }

            return Ok(productDto);
        }

        [HttpGet]

        [Route("api/products/pagination")]
        public async Task<IActionResult> GetProductsWithPagination (string name, int pageNumber = 1) {
            int pageSize = 8;
            try {
                var result = await _productRepository.GetProductsWithPagination(name, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }


}
