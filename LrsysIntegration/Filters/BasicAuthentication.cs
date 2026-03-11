using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace LrsysIntegration.Filters
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string authenticationString = actionContext.Request.Headers.Authorization.Parameter;
                string decodeAuthenticationString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
                string[] userpwd = decodeAuthenticationString.Split(':');
                string username = userpwd[0];
                string password = userpwd[1];
                if (DataLogic.CheckBasicAuthentication.ValidateUser(username, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    actionContext.Response.Content = new StringContent("Invalid username or password!");
                }
            }
        }
    }
}