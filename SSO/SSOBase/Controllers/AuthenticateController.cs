using SSOBase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace SSOBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("get-code")]
        public async Task<IActionResult> GetTokenAsync([FromForm] string client_id,
                                                       [FromForm] string response_type,
                                                       [FromForm] string email,
                                                       [FromForm] string password)
        {
            var client = _context.Clients.Where(b => b.ClientId == client_id).FirstOrDefault();
            if (client == null)
            {
                return BadRequest(new { message = "Unknown client" });
            }
            if (response_type != "code")
            {
                return BadRequest(new { message = "Unknown response type" });
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
                    CodeExpirationTime = codeExpirationTime
                };
                _context.Add(data);
                await _context.SaveChangesAsync();
                return Ok(new { code = code });
            }
            else
            {
                return BadRequest(new { message = "Invalid login or password" });
            }

        }

        [HttpPost]
        [Route("get-token")]
        public async Task<IActionResult> Login([FromForm] string client_id,
                                               [FromForm] string code,
                                               [FromForm] string grant_type,
                                               [FromForm] string client_secret)
        {
            if (grant_type != "authorization_code")
            {
                return BadRequest(new { message = "Unknown grant type" });
            }

            var client = _context.Clients.Where(b => b.ClientId == client_id).FirstOrDefault();
            if (client == null || (client != null && client.ClientSecret != client_secret))
            {
                return BadRequest(new { message = "Invalid client" });
            }
            var data = _context.Datas.Where(b => b.Code == code).FirstOrDefault();
            if (data == null)
            {
                return BadRequest(new { message = "Invalid code" });
            }
            if (data.CodeExpirationTime <= DateTime.UtcNow)
            {
                return BadRequest(new { message = "Code expired" });
            }
            _context.Datas.Remove(data);
            await _context.SaveChangesAsync();
            var user = await _userManager.FindByEmailAsync(data.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Unknown user" });
            }
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var access_token = tokenHandler.WriteToken(token);
            return Ok(new { access_token, token_type = "bearer" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] string client_id,
                                               [FromForm] string code,
                                               [FromForm] string redirect_uri,
                                               [FromForm] string grant_type,
                                               [FromForm] string client_secret)
        {
            if (grant_type != "authorization_code")
            {
                return BadRequest(new { message = "Unknown grant type" });
            }

            var client = _context.Clients.Where(b => b.ClientId == client_id).FirstOrDefault();
            if (client == null || (client != null && client.ClientSecret != client_secret))
            {
                return BadRequest(new { message = "Invalid client" });
            }
            var data = _context.Datas.Where(b => b.Code == code).FirstOrDefault();
            if (data == null)
            {
                return BadRequest(new { message = "Invalid code" });
            }
            if (data.RedirectUri != redirect_uri)
            {
                return BadRequest(new { message = "Invalid redirect uri" });
            }
            if (data.CodeExpirationTime <= DateTime.UtcNow)
            {
                return BadRequest(new { message = "Code expired" });
            }
            _context.Datas.Remove(data);
            await _context.SaveChangesAsync();
            var user = await _userManager.FindByEmailAsync(data.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Unknown user" });
            }
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var access_token = tokenHandler.WriteToken(token);
            return Ok(new { access_token, token_type = "bearer" });
        }

        [Route("all")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> All()
        {
            var email = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            if (!await _userManager.IsInRoleAsync(user, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            return Ok(new { users = _userManager.Users, roles=_roleManager.Roles, claims=claims });
        }

        [Route("clients-all")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> ClientsAll()
        {
            var email = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);
            if (!await _userManager.IsInRoleAsync(user, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            return Ok(new { clients = _context.Clients });
        }

        [Route("me")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> Me()
        {
            var email = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);
            if (!await _userManager.IsInRoleAsync(user, UserRoles.User))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            return Ok(new { id = user.Id });
        }

        [Route("claims")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> Claims()
        {
            var email = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);
            if (!await _userManager.IsInRoleAsync(user, UserRoles.User))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            var claims = await _userManager.GetClaimsAsync(user);
            return Ok(new { claims = claims });
        }

        [Route("user")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> UserInfo(string id)
        {
            var selfEmail = User.Identity.Name;
            var selfUser = await _userManager.FindByEmailAsync(selfEmail);
            if (!await _userManager.IsInRoleAsync(selfUser, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var superuser = await _userManager.FindByEmailAsync("Superuser");
            var neededClaims = await _userManager.GetClaimsAsync(superuser);
            foreach(var claim in neededClaims)
            {
                if (claims.Where(b => b.Type == claim.Type).FirstOrDefault() == null)
                {
                    var newClaim = new Claim(claim.Type, "");
                    await _userManager.AddClaimAsync(user, newClaim);
                }
            }
            claims = await _userManager.GetClaimsAsync(user);
            return Ok(new { user = user, roles = roles, claims = claims });
        }

        [Route("client")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> ClientInfo(string client_id)
        {
            var selfEmail = User.Identity.Name;
            var selfUser = await _userManager.FindByEmailAsync(selfEmail);
            if (!await _userManager.IsInRoleAsync(selfUser, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            var client = _context.Clients.Where(b => b.ClientId == client_id).FirstOrDefault();
            return Ok(new { client = client });
        }

        [Route("edit")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> EditUser(string user_info)
        {
            var selfEmail = User.Identity.Name;
            var selfUser = await _userManager.FindByEmailAsync(selfEmail);
            if (!await _userManager.IsInRoleAsync(selfUser, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            if (user_info == null)
            {
                return BadRequest(new { message = "User can not be empty" });
            }
            UserModel model = JsonConvert.DeserializeObject<UserModel>(user_info);
            List<string> userRoles;
            if (model.roles != null)
            {
                userRoles = model.roles.Split(", ").ToList();
            } else
            {
                userRoles = new List<string>();
                userRoles.Add(UserRoles.User);
            }
            var user = await _userManager.FindByIdAsync(model.id);
            await _userManager.RemoveFromRolesAsync(user, UserRoles.GetAllAdminRoles());
            foreach (var role in userRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest(new { message = "Invalid roles" });
                } else if (role != UserRoles.User)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            user.Name = model.name;
            user.Email = model.email;
            user.Age = model.age;
            await _userManager.UpdateAsync(user);

            var oldClaims = await _userManager.GetClaimsAsync(user);
            await _userManager.RemoveClaimsAsync(user, oldClaims);
            foreach (var claimPair in model.additionalInfo)
            {
                var claim = new Claim(claimPair.Key, claimPair.Value);
                await _userManager.AddClaimAsync(user, claim);
            }

            return Ok();
        }

        [Route("create")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> CreateUser(string user_info)
        {
            var selfEmail = User.Identity.Name;
            var selfUser = await _userManager.FindByEmailAsync(selfEmail);
            if (!await _userManager.IsInRoleAsync(selfUser, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            if (user_info == null)
            {
                return BadRequest(new { message = "User can not be empty" });
            }
            UserModel model = JsonConvert.DeserializeObject<UserModel>(user_info);
            List<string> userRoles;
            if (model.roles != null)
            {
                userRoles = model.roles.Split(", ").ToList();
            }
            else
            {
                userRoles = new List<string>();
                userRoles.Add(UserRoles.User);
            }
            foreach (var role in userRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest(new { message = "Invalid roles" });
                }
            }

            var user = new ApplicationUser()
            {
                Name = model.name,
                Email = model.email,
                Age = model.age,
                UserName = model.email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.password);
            if (!result.Succeeded)
                return BadRequest(new { message = "User creation failed! Please check user details and try again." });

            await _userManager.AddToRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            foreach(var claimPair in model.additionalInfo)
            {
                var claim = new Claim(claimPair.Key, claimPair.Value);
                await _userManager.AddClaimAsync(user, claim);
            }

            return Ok();
        }

        [Route("client-create")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> CreateClient(string client_info)
        {
            var selfEmail = User.Identity.Name;
            var selfUser = await _userManager.FindByEmailAsync(selfEmail);
            if (!await _userManager.IsInRoleAsync(selfUser, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            if (client_info == null)
            {
                return BadRequest(new { message = "Client can not be empty" });
            }
            ClientModel model = JsonConvert.DeserializeObject<ClientModel>(client_info);

            var client = new Client()
            {
                ClientId = model.clientId,
                ClientSecret = model.clientSecret
            };
            _context.Add(client);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("delete")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var selfEmail = User.Identity.Name;
            var selfUser = await _userManager.FindByEmailAsync(selfEmail);
            if (!await _userManager.IsInRoleAsync(selfUser, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return Ok();
        }

        [Route("client-delete")]
        [Authorize(Policy = "ValidAccessToken")]
        public async Task<IActionResult> DeleteClient(string client_id)
        {
            var selfEmail = User.Identity.Name;
            var selfUser = await _userManager.FindByEmailAsync(selfEmail);
            if (!await _userManager.IsInRoleAsync(selfUser, UserRoles.Administrator))
            {
                return BadRequest(new { message = "Permission denied" });
            }
            var client = _context.Clients.Where(b => b.ClientId == client_id).FirstOrDefault();
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
