using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SSOBase.Auth;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SSOBase.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index(string client_id, string response_type, string redirect_uri)
        {
            var client = _context.Clients.Where(b => b.ClientId == client_id).FirstOrDefault();
            if (client == null)
            {
                return Redirect(redirect_uri + "?error=unknown_client");
            }
            if (response_type != "code")
            {
                return Redirect(redirect_uri + "?error=unknown_response_type");
            }
            return View();
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> IndexAsync(string client_id, string response_type, string redirect_uri, [FromForm] string email, [FromForm] string password)
        {
            var client = _context.Clients.Where(b => b.ClientId == client_id).FirstOrDefault();
            if (client == null)
            {
                return Redirect(redirect_uri + "?error=unknown_client");
            }
            if (response_type != "code")
            {
                return Redirect(redirect_uri + "?error=unknown_response_type");
            }
            LoginModel loginModel = new LoginModel()
            {
                Email = email,
                Password = password
            };
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                string code = Guid.NewGuid().ToString();
                DateTime codeExpirationTime = DateTime.UtcNow.AddMinutes(5);
                AuthorizationData data = new()
                {
                    Code = code,
                    Email = email,
                    RedirectUri = redirect_uri,
                    CodeExpirationTime = codeExpirationTime
                };
                _context.Add(data);
                await _context.SaveChangesAsync();
                return Redirect(redirect_uri + "?code=" + code);
                return Content("Login success");
            }
            else
            {
                ViewData["Message"] = "Invalid login or password";
                return View();
            }

        }
    }
}
