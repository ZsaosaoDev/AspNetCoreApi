namespace Asp.NETCoreApi.Data {
    public class Product {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public float Discount { get; set; }

        public int ProductCategoryId { get; set; } // Foreign key
        public ProductCategory ProductCategory { get; set; } // Navigation property

        public ICollection<Color> Colors { get; set; } = new List<Color>();
    }
}