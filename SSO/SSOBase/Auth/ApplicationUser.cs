using Microsoft.AspNetCore.Identity;


namespace SSOBase.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }

        public int? Age { get; set; }

        public string? Code { get; set; }

    }
}
