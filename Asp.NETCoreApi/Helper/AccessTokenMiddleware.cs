namespace Asp.NETCoreApi.Helper {
    public class AccessTokenMiddleware {
        private readonly RequestDelegate _next;

        public AccessTokenMiddleware (RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context) {
            // Lấy access token từ cookie
            var accessToken = context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(accessToken)) {
                // Thiết lập vào header Authorization để tiện dùng trong các phương thức xác thực JWT
                context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            await _next(context);
        }
    }
}
