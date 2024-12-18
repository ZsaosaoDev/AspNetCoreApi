using System.ComponentModel.DataAnnotations;

namespace Asp.NETCoreApi.Dto {
    public class ProductDto {
        public int ProductDtoId { get; set; }
        [Required]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public float Discount { get; set; }


        public int CategoryProductDtoId { get; set; }

        [Required]
        public List<ColorDto> ColorDtos { get; set; }

    }
}