using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SSOBase.Auth;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SSOBase.Controllers
{
    [Route("register")]
    public class RegisterController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterController(UserManager<ApplicationUser> userManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> IndexAsync()
        {
            var superuser = await _userManager.FindByEmailAsync("Superuser");
            var claims = await _userManager.GetClaimsAsync(superuser);
            var model = new RegisterModel();
            ViewBag.NeededClaims = claims;

            foreach(var claim in claims)
            {
                model.AdditionalInfo.Add(claim.Type, "");
            }

            return View(model);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> IndexAsync(RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                ViewData["Message"] = "User with this email already exists!";
                return View();
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Age = model.Age
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ViewData["Message"] = "User creation failed! Please check user details and try again.";
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            foreach(var info in model.AdditionalInfo)
            {
                var claim = new Claim(info.Key, info.Value);
                await _userManager.AddClaimAsync(user, claim);
            }

            return Content("Register success");

        }
    }
}
