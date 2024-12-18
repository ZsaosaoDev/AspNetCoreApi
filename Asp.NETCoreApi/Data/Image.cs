namespace Asp.NETCoreApi.Data {
    public class Image {
        public int ImageId { get; set; }
        public byte[] Data { get; set; }

        public int ColorId { get; set; } // Foreign key
        public Color Color { get; set; } // Navigation property
    }
}
