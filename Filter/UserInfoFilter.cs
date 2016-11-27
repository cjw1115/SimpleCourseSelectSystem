using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace 选课系统.Filter
{
    public class UserInfoFilter:ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult != null)
            {
                var model = viewResult.Model as ViewModel.ViewModelBase;
                if (model != null)
                {
                    BLL.UserinfoBLL bll = new BLL.UserinfoBLL();
                    var userinfo=bll.GetUserInfo(filterContext.HttpContext.User.Identity.Name);
                    model.User = userinfo;
                }
            }
        }
    }
}