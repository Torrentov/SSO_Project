using Microsoft.AspNetCore.Mvc;

namespace UserCRUD.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Redirect(_configuration["Constants:SSO"] + 
                            "/login/?client_id=e74dfse9-s45t-efw5-1hey-548werbgth9a&response_type=code&redirect_uri=" +
                            _configuration["Constants:Self"] +
                            "/users/api/login");
        }
    }
}