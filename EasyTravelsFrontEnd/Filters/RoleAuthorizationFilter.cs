using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EasyTravelsFrontEnd.Filters
{
    public class RoleAuthorizationFilter : IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorizationFilter(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRoleId = context.HttpContext.Session.GetString("RoleId");

            if (string.IsNullOrEmpty(userRoleId) || !_allowedRoles.Contains(int.Parse(userRoleId)))
            {
                // Redireciona para a página de acesso negado ou login
                context.Result = new RedirectToActionResult("Login", "Auth", new { error = "Acesso Negado" });
            }
        }
    }
}
