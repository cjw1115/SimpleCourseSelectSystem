using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using 选课系统.BLL;
using 选课系统.Model;

namespace 选课系统.Controllers
{
    public class CenterController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(int? id)
        {
            ViewModel.RegistVM vm = new ViewModel.RegistVM { Code = 0, Msg = "登录失败" };
            JsonResult json = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = vm };

            string username = Request["tbUsername"] == null ? null : Request["tbUsername"].ToString().Trim();
            string password = Request["tbPassword"] == null ? null : Request["tbPassword"].ToString().Trim();
            string usertypeStr = Request["btnRadio"] == null ? null : Request["btnRadio"];
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                vm.Msg = "用户名或密码不能为空！";
                return json;
            }
          
            UserType userType;
            if (usertypeStr.ToLower().Contains("student"))
            {
                userType = UserType.Student;
            }
            else if (usertypeStr.ToLower().Contains("mentor"))
            {
                userType = UserType.Teacher;
            }
            else
            {
                vm.Msg = "非法请求！";
                return json;
            }

            BLL.UserinfoBLL userinfoBLL = new BLL.UserinfoBLL();
            User userinfo = userinfoBLL.GetUserInfo(username);
            if (userinfo == null)
            {
                vm.Msg = "该用户不存在！";
                return json;

            }
            else
            {
                if (userinfo.Password.Equals(password)&&userinfo.UserType==userType)
                {
                    //string role = userType.ToString();
                    
                    //Session["username"] = username;

                    FormsAuthentication.SetAuthCookie(username, false);
                    
                    //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddSeconds(60), true, "student",FormsAuthentication.FormsCookiePath);
                    //var encryptTicket=FormsAuthentication.Encrypt(ticket);
                    //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                    //Response.Cookies.Add(cookie);
                    //登录成功导航至其他页面
                    //Response.Write("<script>alert('登录成功！')</script>");
                    vm.Code = 1;
                    vm.Msg = "登录成功";
                    return json;
                }
                else
                {
                    Session["username"] = null;
                    vm.Msg = "密码错误！";
                    return json;
                }
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult Regist()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Regist(int? id)
        {
            ViewModel.RegistVM vm = new ViewModel.RegistVM { Code = 0, Msg = "注册失败" };
            JsonResult json = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = vm };
            User userinfo = null;
            if (ModelState.IsValid)
            {
                userinfo = new Model.User();
                string userTypeStr = Request["cbUserType"] == null ? null : Request["cbUserType"].ToString();
                UserType userType;
                if (userTypeStr.ToLower().Contains("student"))
                {
                    userType = UserType.Student;
                }
                else if (userTypeStr.ToLower().Contains("teacher"))
                {
                    vm.Msg = "教师注册请联系管理员！";
                    return json;
                }
                else
                {
                    vm.Msg = "请求非法！";
                    return json;
                }

                //所有人必填信息
                userinfo.Name= Request["tbName"] == null ? null : Request["tbName"].ToString().Trim();
                userinfo.Gender = Request["cbSex"] == null ? null : Request["cbSex"].ToString().Trim();
                userinfo.Username = Request["tbUsername"] == null ? null : Request["tbUsername"].ToString().Trim();
                userinfo.Password = Request["tbPassword"] == null ? null : Request["tbPassword"].ToString().Trim();
                userinfo.UserType = userType;
                userinfo.Contact = Request["tbContract"] == null ? null : Request["tbContract"].ToString().Trim();
                string passwordRe= Request["tbPasswordRe"] == null ? null : Request["tbPasswordRe"].ToString().Trim();
                if (string.IsNullOrEmpty(userinfo.Name) ||
                    string.IsNullOrEmpty(userinfo.Username) ||
                    string.IsNullOrEmpty(userinfo.Password) ||
                    string.IsNullOrEmpty(passwordRe) ||
                    string.IsNullOrEmpty(userinfo.Gender))
                {
                    vm.Msg = "信息不完整！";
                    return json;
                }
                if (!userinfo.Password.Equals(passwordRe))
                {
                    vm.Msg = "前后密码不一致！";
                    return json;
                }

                Student studentInfo = null;
                if (userType == UserType.Student)
                {
                    studentInfo = new Student();
                    
                    studentInfo.Major = Request["cbFangxiang"] == null ? null : Request["cbFangxiang"].ToString().Trim();
                    studentInfo.Mentor = Request["tbTeacherName"] == null ? null : Request["tbTeacherName"].ToString().Trim();
                    //studentInfo.Score = string.IsNullOrEmpty(Request["tbScore"]) ? 0 : double.Parse( Request["tbScore"].ToString().Trim());
                    studentInfo.PersonInfo = Request["tbStuInfo"] == null ? null : Request["tbStuInfo"].ToString().Trim();
                    if (string.IsNullOrEmpty(studentInfo.Major) ||
                        string.IsNullOrEmpty(studentInfo.Mentor) )
                    {
                        vm.Msg = "信息不完整！";
                        return json;
                    }
                    studentInfo.UserID = userinfo.Username;
                    userinfo.Student = studentInfo;
                }
                //注册
                BLL.UserinfoBLL userinfoBLL = new BLL.UserinfoBLL();
                if (userinfoBLL.HasExist(userinfo.Username))
                {
                    vm.Msg = "当前学号已注册！";
                    return json;
                }
                if (true == userinfoBLL.Regist(userinfo))
                {
                    vm.Msg = "注册成功！";
                    vm.Code = 1;
                    return json;
                }
                else
                {
                    vm.Msg = "注册出现问题，联系管理员！";
                    return json;
                }

            }
            return json;
        }

        [Authorize(Roles ="Student")]
        [HttpGet]
        public ActionResult Info()
        {
            UserinfoBLL userBll = new UserinfoBLL();
            var userinfo=userBll.GetUserInfo(User.Identity.Name);
            ViewModel.ViewModelBase vm = new ViewModel.ViewModelBase();
            vm.User = userinfo;
            return View(vm);
        }
        [Authorize(Roles = "Student")]
        [HttpPost]
        public ActionResult Info(User userinfo)
        {
            
            if (userinfo.Username.Equals(User.Identity.Name))
            {
                UserinfoBLL userBll = new UserinfoBLL();
                var userinfoOld = userBll.GetUserInfo(User.Identity.Name);
                userinfoOld.Name = userinfo.Name;
                userinfoOld.Gender = userinfo.Gender;
                userinfoOld.Contact = userinfo.Contact;
                userinfoOld.Student.Major = userinfo.Student.Major;
                userinfoOld.Student.PersonInfo = userinfo.Student.PersonInfo;
                userinfoOld.Student.Mentor = userinfo.Student.Mentor;
                //userinfoOld.Student.Score = userinfo.Student.Score;

                if (userBll.Change(userinfoOld))
                {
                    Response.Write("<script>alert('修改成功')</script>");
                    return RedirectToAction("Info");
                }
            }
            Response.Write("<script>alert('数据非法')</script>");
            ViewModel.ViewModelBase vm = new ViewModel.ViewModelBase();
            vm.User = userinfo;
            return View(vm);
        }

        [Authorize(Roles = "Student")]
        public ActionResult ChangePwd()
        {
            UserinfoBLL userBll = new UserinfoBLL();
            var userinfo = userBll.GetUserInfo(User.Identity.Name);
            ViewModel.ViewModelBase vm = new ViewModel.ViewModelBase();
            vm.User = userinfo;
            return View(vm);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public ActionResult ChangePwd(string oldPwd,string newPwd,string rePwd)
        {
            ViewModel.ViewModelBase vm = new ViewModel.ViewModelBase();
            
            UserinfoBLL userBll = new UserinfoBLL();
            var userinfo = userBll.GetUserInfo(User.Identity.Name);
            vm.User = userinfo;
            if (!newPwd.Equals(rePwd))
            {
                Response.Write("<script>alert('前后密码不一致')</script>");
                return View(vm);
            }
            if (!userinfo.Password.Equals(oldPwd))
            {
                Response.Write("<script>alert('旧密码输入错误')</script>");
                return View(vm);
            }
            userinfo.Password = newPwd;
            bool re=userBll.Change(userinfo);
            
            vm.User = userinfo;
            
            if (re)
            {
                Response.Write("<script>alert('修改成功')</script>");
                return View(vm);
            }
            else
            {
                Response.Write("<script>alert('修改失败')</script>");
                return View(vm);
            }
        }
    }
}