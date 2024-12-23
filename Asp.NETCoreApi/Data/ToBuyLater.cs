namespace Asp.NETCoreApi.Data {
    public class ToBuyLater {
        public int ToBuyLaterId { get; set; }

        public int SizeId { get; set; } // Foreign key

        public int Quantity { get; set; }

        public Size Size { get; set; } // Navigation property

        public string UserId { get; set; } // Foreign key

        //public ApplicationUser User { get; set; } // Navigation property for User (assuming ApplicationUser is the user model)


    }
}
