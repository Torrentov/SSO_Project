using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SSOBase.Auth
{
    public class AuthorizationData
    {
        [Key]
        public string? Code { get; set; }
        public DateTime? CodeExpirationTime { get; set; }
        public string? Email { get; set; }
        public string? RedirectUri { get; set; }

    }
}
