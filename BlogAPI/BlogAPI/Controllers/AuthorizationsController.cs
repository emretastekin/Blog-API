using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthorizationsController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

        }


        [HttpPost("Login")]
        public ActionResult Login(string userName, string password)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;
            //Microsoft.AspNetCore.Identity.SignInResult signInResult;

            if (applicationUser != null)
            {
                var signInResult = _userManager.CheckPasswordAsync(applicationUser, password).Result;
                if (signInResult)
                {
                    var token = GenerateJwtToken(applicationUser);
                    return Ok(new { Token = token });
                }
            }
            return Unauthorized();
        }

        private object GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        // Diğer claim'leri ekleme ihtiyacınıza göre buraya ekleyebilirsiniz
    };

            // Kullanıcının rollerini ekleyin
            var userRoles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Kullanıcının diğer talep bilgilerini ekleyin
            var userClaims = _userManager.GetClaimsAsync(user).Result;
            claims.AddRange(userClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(1), // Token geçerlilik süresi
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Token'i JSON formatında döndürmek için bir anonymous object kullanın
            return new
            {
                token = tokenHandler.WriteToken(token),
                expiration = token.ValidTo
            };
        }

        [Authorize]
        [HttpGet("Logout")]
        public ActionResult LogOut()
        {
            _signInManager.SignOutAsync();
            return Ok();
        }

    }
}