using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace 选课系统
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            AuthorizeRequest += MvcApplication_AuthorizeRequest;
        }

        private void MvcApplication_AuthorizeRequest(object sender, EventArgs e)
        {
            var identity = Context.User.Identity;
            if (identity.IsAuthenticated)
            {
                var roles = BLL.UserinfoBLL.GetUserRoles(identity.Name);
                Context.User = new GenericPrincipal(identity, roles);
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
        }
    }
}
