using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;


namespace App.Components.Utilities.JWT_Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JWTAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private string _permission;        
        public JWTAuthorizeAttribute(string permission=null)
        {
            _permission = permission;

        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserClient)context.HttpContext.Items["UserClient"];
            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(new { ErrorMessages = "Unauthenticated  Access!!" });
            }
            else if (!string.IsNullOrEmpty(_permission) && !user.Permissions.Contains(_permission))
            {
                context.Result = new UnauthorizedObjectResult(new { ErrorMessages = "Unauthorized Access!!" });
            }


        }
    }
}
