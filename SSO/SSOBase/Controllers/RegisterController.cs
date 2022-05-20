using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SSOBase.Auth;
using System.Net;
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
        public IActionResult Index()
        {
            return View();
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
            return Content("Register success");

        }
    }
}
