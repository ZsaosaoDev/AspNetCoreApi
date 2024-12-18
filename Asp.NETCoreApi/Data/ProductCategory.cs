namespace Asp.NETCoreApi.Data {
    public class ProductCategory {
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Picture { get; set; }

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();


    }
}
