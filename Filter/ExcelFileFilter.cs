using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace 选课系统.Filter
{
   
    public class ExcelFileFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.ContentType = "application/ms-excel";
            filterContext.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=excelfile.xls");
            filterContext.HttpContext.Response.ContentEncoding = Encoding.GetEncoding("gb2312");
        }
    }
}