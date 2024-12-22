using Asp.NETCoreApi.DTOs;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NETCoreApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly IAccountRepository _accountRepository;

        public AccountController (IAccountRepository accountRepository) {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Registers a new user and returns their role if successful.
        /// </summary>
        [HttpPost("SignUp")]
        public async Task<ActionResult<string>> SignUp ([FromBody] SignUpDto model) {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input. Please ensure all fields are valid.");

            var result = await _accountRepository.SignUpAsync(model);

            if (result.StatusCode != 200) {
                return BadRequest(result.Message);
            }

            // Set HTTP-only cookies for tokens
            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(result.Role);
        }

        /// <summary>
        /// Logs in a user and returns their role if successful.
        /// </summary>
        [HttpPost("SignIn")]
        public async Task<ActionResult<string>> SignIn ([FromBody] SignInDto model) {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input. Please ensure all fields are valid.");

            var result = await _accountRepository.SignInAsync(model);

            if (result.StatusCode != 200) {
                return Unauthorized(result.Message);
            }

            // Set HTTP-only cookies for tokens
            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(result.Role);
        }

        /// <summary>
        /// Refreshes tokens and returns the user's role if successful.
        /// </summary>
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<string>> RefreshToken () {
            // Get the refresh token from cookies
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken)) {
                return BadRequest("Refresh token is required.");
            }

            // Attempt to refresh tokens using the provided refresh token
            var result = await _accountRepository.RefreshTokenAsync(refreshToken);

            if (result.StatusCode != 200) { // Assuming 200 is success
                return Unauthorized(result.Message);
            }

            // Update cookies with new tokens
            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(result.Role);
        }


        /// <summary>
        /// Logs out a user and clears refresh tokens and cookies.
        /// </summary>
        [Authorize]
        [HttpPost("Logout")]
        public async Task<ActionResult<string>> Logout () {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Unable to identify user.");

            await _accountRepository.LogoutAsync(userId);

            // Clear cookies
            Response.Cookies.Delete("accessToken", new CookieOptions {
                Secure = true,
                SameSite = SameSiteMode.None
            });

            Response.Cookies.Delete("refreshToken", new CookieOptions {
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok("Logged out successfully.");
        }
    }
}
