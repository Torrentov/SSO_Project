using System.ComponentModel.DataAnnotations;


namespace SSOBase.Auth
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        public string? Password { get; set; }

        public Dictionary<string, string>? AdditionalInfo { get; set; }

        public RegisterModel()
        {
            AdditionalInfo = new Dictionary<string, string>();
        }
    }
}
