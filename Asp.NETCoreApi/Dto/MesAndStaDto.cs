namespace Asp.NETCoreApi.Dto {
    public class MesAndStaDto {
        public string Message { get; set; }
        public int Status { get; set; }

        public MesAndStaDto () { }

        public MesAndStaDto (string message, int status = 200) {
            Message = message;
            Status = status;
        }
    }
}
