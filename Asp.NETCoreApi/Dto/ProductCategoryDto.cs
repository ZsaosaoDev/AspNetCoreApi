namespace Asp.NETCoreApi.Dto {
    public class ProductCategoryDto {

        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Picture { get; set; }
        public List<ProductDto> ProductDtos { get; set; }
    }
}
