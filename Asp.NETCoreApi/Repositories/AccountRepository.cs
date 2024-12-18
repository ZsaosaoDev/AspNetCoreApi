using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.DTOs;
using Asp.NETCoreApi.Helper;
using Asp.NETCoreApi.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Asp.NETCoreApi.Repositories {
    public class AccountRepository : IAccountRepository {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;

        public AccountRepository (
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            MyDbContext context) {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        private string GenerateRefreshToken () {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string GenerateAccessToken (List<Claim> claims) {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(20),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<LogDto> SignInAsync (SignInDto model) {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new LogDto("Invalid credentials.", 401);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var role = await _userManager.GetRolesAsync(user);
            if (role.Any()) {
                claims.Add(new Claim(ClaimTypes.Role, role.First())); // Add only the first role
            }

            var accessToken = GenerateAccessToken(claims);
            var refreshToken = new RefreshToken {
                Token = GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new LogDto("Login successful", role.FirstOrDefault(), accessToken, refreshToken.Token);
        }

        public async Task<LogDto> RefreshTokenAsync (string refreshToken) {
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (storedToken == null || storedToken.ExpiryDate <= DateTime.UtcNow)
                return new LogDto("Invalid or expired refresh token.");

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return new LogDto("User not found.");

            _context.RefreshTokens.Remove(storedToken);

            var newRefreshToken = new RefreshToken {
                Token = GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            var accessToken = GenerateAccessToken(claims);
            return new LogDto("Token refreshed successfully", roles.FirstOrDefault(), accessToken, newRefreshToken.Token);
        }

        public async Task<LogDto> SignUpAsync (SignUpDto model) {

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null) {
                return new LogDto("The email is already registered.", 400);
            }

            var user = new ApplicationUser { Email = model.Email, UserName = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return new LogDto(string.Join("; ", result.Errors.Select(e => e.Description)), 400);

            if (!await _roleManager.RoleExistsAsync(AppRole.Customer))
                await _roleManager.CreateAsync(new IdentityRole(AppRole.Customer));

            await _userManager.AddToRoleAsync(user, AppRole.Customer);
            return await SignInAsync(new SignInDto { Email = model.Email, Password = model.Password });
        }

        public async Task LogoutAsync (string userId) {
            var tokens = _context.RefreshTokens.Where(rt => rt.UserId == userId);
            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetUserRoleAsync (string email) {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "No Role Assigned";
        }
    }
}
