namespace Asp.NETCoreApi.Dto {
    public class ColorDto {

        public int ColorDtoId { get; set; }
        public string Name { get; set; }

        public string HexCode { get; set; }

        public int ProductId { get; set; }



        public List<SizeDto> SizeDtos { get; set; }

        public List<ImageDto> ImageDtos { get; set; }
        public bool IsPreviouslySelected { get; internal set; }
        public int Quantity { get; internal set; }
    }
}