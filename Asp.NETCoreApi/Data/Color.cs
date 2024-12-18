namespace Asp.NETCoreApi.Data {
    public class Color {

        public int ColorId { get; set; }
        public string Name { get; set; }
        public string HexCode { get; set; }

        public int ProductId { get; set; } // Foreign key
        public Product Product { get; set; } // Navigation property

        public ICollection<Size> Sizes { get; set; } = new List<Size>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
