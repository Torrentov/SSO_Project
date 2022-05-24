using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class User
    {
        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; }
        [Required(ErrorMessage = "Age is required.")]
        public int age { get; set; }
        public string? code { get; set; }
        public string? id { get; set; }
        public string? userName { get; set; }
        public string? normalizedUserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string email { get; set; }
        public string? normalizedEmail { get; set; }
        public bool? emailConfirmed { get; set; }
        public string? passwordHash { get; set; }
        public string? securityStamp { get; set; }
        public string? concurrencyStamp { get; set; }
        public object? phoneNumber { get; set; }
        public bool? phoneNumberConfirmed { get; set; }
        public bool? twoFactorEnabled { get; set; }
        public object? lockoutEnd { get; set; }
        public bool? lockoutEnabled { get; set; }
        public int? accessFailedCount { get; set; }

        [Required(ErrorMessage = "Role list is required.")]
        public string roles { get; set; }
        
        public string? password { get; set; }
        public Dictionary<string, string> additionalInfo { get; set; }

        public User()
        {
            additionalInfo = new Dictionary<string, string>();
        }

    }

    public class Claim
    {
        public string issuer { get; set; }
        public string originalIssuer { get; set; }
        public Properties properties { get; set; }
        public object subject { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string valueType { get; set; }
    }

    public class Properties
    {
    }

    public class Role
    {
        public string id { get; set; }
        public string name { get; set; }
        public string normalizedName { get; set; }
        public string concurrencyStamp { get; set; }
    }

    public class Root
    {
        public List<User> users { get; set; }
        public List<Role> roles { get; set; }
    }

    public class UserId
    {
        public string id { get; set; }
    }

    public class ClaimsRoot
    {
        public List<Claim> claims { get; set; }
    }

    public class UserRoot
    {
        public User user { get; set; }
        public List<string> roles { get; set; }
        public string id { get; set; }
        public List<Claim> claims { get; set; }
    }

    public class UserModel
    {
        public string id { get;  set; }
        public string email { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string roles { get; set; }
        public string password { get; set; }
        public Dictionary<string, string> additionalInfo { get; set; }

        public UserModel(User user)
        {
            id = user.id;
            email = user.email;
            name = user.name;
            age = user.age;
            roles = user.roles;
            password = user.password;
            additionalInfo = user.additionalInfo;
        }
    }


}
