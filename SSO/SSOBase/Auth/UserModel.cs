namespace SSOBase.Auth
{
    public class UserModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string roles { get; set; }
        public string? password { get; set; }
        public Dictionary<string, string> additionalInfo { get; set; }
        public Dictionary<string, string>? VisibleClaims { get; set; }
    }
}
