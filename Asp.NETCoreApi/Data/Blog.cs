using System.ComponentModel.DataAnnotations;

namespace Asp.NETCoreApi.Data {
    public class Blog {

        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        // Lưu ảnh dưới dạng byte[] (Binary Data)
        public byte[] ImageData { get; set; }

        [MaxLength(50)]
        public string ImageMimeType { get; set; } // Lưu loại file (jpg, png,...)



    }
}
