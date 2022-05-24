using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SSOBase.Auth;
using System.Security.Claims;

namespace SSOBase.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public HomeController(UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        private async Task<int> CreateRolesAsync()
        {
            var userRoles = UserRoles.GetAllRoles();
            foreach (var role in userRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            return 1;
        }

        private async Task<int> CreateSuperuserAsync()
        {
            var userExists = await _userManager.FindByEmailAsync("Superuser");
            if (userExists != null)
            {
                await _userManager.DeleteAsync(userExists);
            }

            ApplicationUser user = new()
            {
                Email = "Superuser",
                UserName = "Superuser",
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = "Superuser",
                Age = 19
            };
            var result = await _userManager.CreateAsync(user, _configuration["Constants:SuperuserPassword"]);
            if (!result.Succeeded)
            {
                return 0;
            }

            var userRoles = UserRoles.GetAllRoles();
            foreach (var role in userRoles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return 1;
        }

        [Route("")]
        public async Task<IActionResult> IndexAsync()
        {
            await CreateRolesAsync();

            await CreateSuperuserAsync();

            var customClaimsArray = _configuration.GetSection("Claims").Get<List<CustomClaim>>();
            var user = await _userManager.FindByEmailAsync("Superuser");
            foreach(var customClaim in customClaimsArray)
            {
                var claim = new Claim(customClaim.Name, "N/A");
                await _userManager.AddClaimAsync(user, claim);
            }

            return View();
        }

        [Route("About")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
    }
}
