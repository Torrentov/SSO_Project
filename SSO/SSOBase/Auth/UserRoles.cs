namespace SSOBase.Auth
{
    public static class UserRoles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";

        public static List<string> GetAllAdminRoles()
        {
            List<string> result = new List<string>();
            result.Add(Administrator);
            return result;
        }

        public static List<string> GetAllRoles()
        {
            List<string> result = GetAllAdminRoles();
            result.Add(User);
            return result;
        }
    }
}
