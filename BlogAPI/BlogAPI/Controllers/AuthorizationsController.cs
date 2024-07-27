using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthorizationsController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login(string userName, string password)
        {
            ApplicationUser applicationUser = await _userManager.FindByNameAsync(userName);
            if (applicationUser != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(applicationUser, password, false, false);
                if (signInResult.Succeeded==true)
                {
                    return Ok();
                }
            }
            return Unauthorized();
        }


        [HttpGet("Logout")]
        public ActionResult LogOut()
        {
            _signInManager.SignOutAsync();
            return Ok();
        }

    }
}
