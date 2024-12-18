namespace Asp.NETCoreApi.Dto {
    public class LogDto {
        public string Message { get; set; }
        public string Role { get; set; }
        public int StatusCode { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public LogDto (string message, int statusCode = 200) {
            Message = message;
            StatusCode = statusCode;
        }

        public LogDto (string message, string role, string accessToken, string refreshToken) {
            Message = message;
            Role = role;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            StatusCode = 200;
        }
    }
}
