using Microsoft.AspNetCore.Mvc;

namespace EasyTravelsFrontEnd.Filters
{
    public class RoleAuthorizeAttribute : TypeFilterAttribute
    {
        public RoleAuthorizeAttribute(params int[] allowedRoles) : base(typeof(RoleAuthorizationFilter))
        {
            Arguments = new object[] { allowedRoles };
        }
    }
}
