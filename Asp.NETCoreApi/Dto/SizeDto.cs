namespace Asp.NETCoreApi.Dto {
    public class SizeDto {

        public int SizeDtoId { get; set; }
        public string Name { get; set; }

        public int Stock { get; set; }

        public int ColorId { get; set; }
        public int Quantity { get; internal set; }
    }
}