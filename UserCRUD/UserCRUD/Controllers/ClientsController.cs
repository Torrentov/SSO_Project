using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using UserCRUD.Data;
using UserCRUD.Models;

namespace UserCRUD.Controllers
{
    [Route("[controller]")]
    public class ClientsController : Controller
    {
        private readonly ApiTokenDbContext _context;
        private readonly IConfiguration _configuration;

        public ClientsController(ApiTokenDbContext context,
                                 IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Clients
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
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/clients-all");
            var usersString = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<ClientsRoot>(usersString);
            if (model == null || model.clients == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }
            return View(model);
        }

        // GET: Clients/Create
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

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                var clientInfo = JsonConvert.SerializeObject(client);
                HttpClient _httpClient = new HttpClient();
                var userId = HttpContext.Session.GetString("id");
                var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
                var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/client-create?client_info=" + clientInfo);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                return Content(messages);
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        [Route("delete")]
        public async Task<IActionResult> Delete(string? clientId)
        {
            if (clientId == null)
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
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/client?client_id=" + clientId);
            var clientRoot = await response.Content.ReadAsAsync<ClientRoot>();
            var client = clientRoot.client;
            if (client == null)
            {
                return Redirect(_configuration["Constants:Self"]);
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string clientId)
        {
            HttpClient _httpClient = new HttpClient();
            var userId = HttpContext.Session.GetString("id");
            var token = _context.Tokens.Where(b => b.id == userId).FirstOrDefault();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
            var response = await _httpClient.GetAsync(_configuration["Constants:SSO"] + "/api/Authenticate/client-delete?client_id=" + clientId);
            return RedirectToAction(nameof(Index));
        }
    }
}
