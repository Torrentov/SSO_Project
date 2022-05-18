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
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> IndexAsync([FromForm] string name, [FromForm] string email, [FromForm] int age, [FromForm] string password)
        {
            if (password.Length < 6)
            {
                ViewData["Message"] = "Too short password, please use minimum 6 symbols";
                return View();
            }
            if (age > 130)
            {
                ViewData["Message"] = "Please enter your real age";
                return View();
            }
            RegisterModel registerModel = new RegisterModel()
            {
                Name = name,
                Email = email,
                Age = age,
                Password = password
            };
            HttpClient _httpClient = new HttpClient();
            var model = JsonSerializer.Serialize(registerModel);
            var requestContent = new StringContent(model, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_configuration["Constants:Self"] + "/api/Authenticate/register", requestContent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Content("Registration success");
            }
            else
            {
                string result = await response.Content.ReadAsStringAsync();
                return Content(result);
            }

        }
    }
}
