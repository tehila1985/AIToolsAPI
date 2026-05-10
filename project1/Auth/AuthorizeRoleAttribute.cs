using Microsoft.AspNetCore.Authorization;

namespace Api.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(string role)
        {
            Roles = role;
        }
    }
}
