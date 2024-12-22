namespace Asp.NETCoreApi.Helper {
    using Microsoft.AspNetCore.Http;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    public class TokenExpirationMiddleware {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenExpirationMiddleware (RequestDelegate next, IConfiguration configuration) {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync (HttpContext context) {
            var accessToken = context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(accessToken)) {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

                try {
                    // Validate token without checking expiration
                    tokenHandler.ValidateToken(accessToken, new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["JWT:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = _configuration["JWT:ValidAudience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = false // Skip expiration validation here
                    }, out SecurityToken validatedToken);

                    var jwtToken = validatedToken as JwtSecurityToken;

                    // Check token expiration manually
                    if (jwtToken != null && jwtToken.ValidTo < DateTime.UtcNow) {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new {
                            message = "Access token expired. Please refresh the token."
                        });
                        return;
                    }
                }
                catch (SecurityTokenException ex) {
                    // Invalid token or other issues
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new {
                        message = "Invalid access token.",
                        details = ex.Message
                    });
                    return;
                }
            }

            // Proceed to the next middleware if token is valid or not provided
            await _next(context);
        }
    }
}
