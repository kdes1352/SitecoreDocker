using Sitecore.Security.Accounts;

namespace AllinaHealth.Framework.Extensions
{
    public static class UserExtensions
    {
        public static bool IsUserBusinessAnalystRole(this User u)
        {
            var role = Role.FromName("sitecore\\AH DigEx BAs");
            return role != null && u.IsInRole(role);
        }
    }
}