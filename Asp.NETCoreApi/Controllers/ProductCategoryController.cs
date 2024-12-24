using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NETCoreApi.Controllers {
    public class ProductCategoryController : Controller {

        private readonly IProductCategoryRepository _productCategoryRepository;

        public ProductCategoryController (IProductCategoryRepository productCategoryRepository) {
            _productCategoryRepository = productCategoryRepository;
        }

        [HttpGet]
        [Route("api/productcategories")]
        public async Task<IActionResult> GetProductCategories () {
            try {
                var result = await _productCategoryRepository.GetAllProductCategories();
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


        [HttpGet("GetProductCategoryByName")]
        public async Task<IActionResult> GetProductCategoryByName (string name) {
            try {
                var result = await _productCategoryRepository.GetProductCategories(name);
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/productcategories/pagination")]
        public async Task<IActionResult> GetProductCategoriesWithPagination (string name, string productName, int pageNumber = 1) {
            int pageSize = 8;
            try {
                var result = await _productCategoryRepository.GetProductsWithPagination(name, pageNumber, pageSize, productName);
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("api/addCategory")]

        public async Task<IActionResult> AddProductCategory ([FromBody] ProductCategoryDto productCategoryDto) {
            try {
                var result = await _productCategoryRepository.AddProductCategory(productCategoryDto);
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
