
namespace Asp.NETCoreApi.Dto {
    public class ImageDto {

        public int ImageDtoId { get; set; }

        // Use byte[] for storing image data
        public byte[] Data { get; set; }


    }
}