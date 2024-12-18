namespace Asp.NETCoreApi.Data {
    public class Size {

        public int SizeId { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }

        public int ColorId { get; set; } // Foreign key
        public Color Color { get; set; } // Navigation property
    }
}
