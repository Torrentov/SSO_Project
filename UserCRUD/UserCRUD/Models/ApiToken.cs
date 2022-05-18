using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class ApiToken
    {
        [Key]
        public string? access_token { get; set; }

        public string? token_type { get; set; }
    }

    public class UserToken
    {
        [Key]
        public string? id { get; set; }

        public string? access_token { get; set; }
        public string? token_type { get; set; }
    }
}
