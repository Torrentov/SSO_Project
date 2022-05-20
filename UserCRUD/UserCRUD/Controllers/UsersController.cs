using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using UserCRUD.Data;
using UserCRUD.Models;
using Newtonsoft.Json;
using System.Net;

namespace UserCRUD.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ApiTokenDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(ApiTokenDbContext context,
                               IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("api/login")]
        public async Task<IActionResult> LoginAsync(string code)
        {
            HttpClient _httpClient = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "redirect_uri", _configuration["Constants:Self"] + "/users/api/login" },
                { "client_id", "e74dfse9-s45t-efw5-1hey-548werbgth9a" },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "client_secret", "8uxNBaD2qXdtQsxQN6Q19MdagyQjVGg3y4UyNq4q" }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync(_configuration["Constants:SSO"] + "/api/Authenticate/login", content);
            if (!response.IsSuccessStatusCode)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            var token = await response.Content.ReadAsAsync<ApiToken>();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
            response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/me");
            var userId = await response.Content.ReadAsAsync<UserId>();
            var id = userId.id;
            var tokenExists = _context.Tokens.Where(b => b.id == id).FirstOrDefault();
            if (tokenExists != null)
            {
                _context.Tokens.Remove(tokenExists);
                await _context.SaveChangesAsync();
            }
            UserToken userToken = new()
            {
                id = id,
                access_token = token.access_token,
                token_type = token.token_type
            };
            _context.Add(userToken);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("id", id);

            return Redirect(_configuration["Constants:Self"] + "/users");
        }

        // GET: Users
        [Route("")]
        public async Task<IActionResult> Index()
        {
            HttpClient _httpClient = new HttpClient();
            var userId = HttpContext.Session.GetString("id");
            var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
            if (token == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/all");
            var usersString = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<Root>(usersString);
            if (model == null || model.users == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            return View(model);
        }

        // GET: Users/Details/5
        [Route("details")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient _httpClient = new HttpClient();
            var userId = HttpContext.Session.GetString("id");
            var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
            if (token == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/user?id=" + id);
            var res = await response.Content.ReadAsStringAsync();
            var user = await response.Content.ReadAsAsync<UserRoot>();
            if (user == null || user.user == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            return View(user);
        }

        // GET: Users/Create
        [Route("create")]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("id");
            var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
            if (token == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                UserModel model = new UserModel(user);
                var userInfo = JsonConvert.SerializeObject(model);
                HttpClient _httpClient = new HttpClient();
                var userId = HttpContext.Session.GetString("id");
                var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
                var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/create?user_info=" + userInfo);
                return RedirectToAction(nameof(Index));
            } else
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        [Route("edit")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient _httpClient = new HttpClient();
            var userId = HttpContext.Session.GetString("id");
            var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
            if (token == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/user?id=" + id);
            var user = await response.Content.ReadAsAsync<UserRoot>();
            if (user == null || user.user == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            string role = "";
            bool first = true;
            foreach (var userRole in user.roles)
            {
                if (first)
                {
                    role += userRole;
                    first = false;
                }
                else
                {
                    role += ", " + userRole;
                }
            }
            User editableUser = user.user;
            editableUser.roles = role;
            return View(editableUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, User user)
        {
            if (id != user.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                UserModel model = new UserModel(user);
                var userInfo = JsonConvert.SerializeObject(model);
                HttpClient _httpClient = new HttpClient();
                var userId = HttpContext.Session.GetString("id");
                var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
                var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/edit?user_info=" + userInfo);
                return RedirectToAction(nameof(Index));
            } else
            {
                var message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
                return Content(message);
            }
            return View(user);
        }

        // GET: Users/Delete/5
        [Route("delete")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient _httpClient = new HttpClient();
            var userId = HttpContext.Session.GetString("id");
            var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
            if (token == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/user?id=" + id);
            var user = await response.Content.ReadAsAsync<UserRoot>();
            if (user == null || user.user == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            HttpClient _httpClient = new HttpClient();
            var userId = HttpContext.Session.GetString("id");
            var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/delete?id=" + id);
            return RedirectToAction(nameof(Index));
        }
    }
}
