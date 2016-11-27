using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 选课系统.Model;
using 选课系统.Filter;
namespace 选课系统.Controllers
{
    
    public class CourseController : Controller
    {
        private class Result
        {
            public int Code { get; set; }
            public string Msg { get; set; }
            public object Param { get; set; }
        }
        [Authorize(Roles = "Student")]
        [UserInfoFilter]
        public ActionResult MainCourse()
        {
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            BLL.UserinfoBLL userBll = new BLL.UserinfoBLL();
            User userinfo = userBll.GetUserInfo(User.Identity.Name);
            var allCourse = courseBll.GetAllCourse();
            var list= allCourse.Where(i => i.Major.Equals(userinfo.Student.Major)).Select(i=>i);
            ViewModel.CourseListVM vm = new ViewModel.CourseListVM();
            vm.CourseList = list;
            
            return View(vm);
        }

        [Authorize(Roles = "Student")]
        [UserInfoFilter]
        [HttpGet]
        public ActionResult OtherCourse()
        {
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            BLL.UserinfoBLL userBll = new BLL.UserinfoBLL();
            User userinfo = userBll.GetUserInfo(User.Identity.Name);
            var allCourse = courseBll.GetAllCourse();
            var list = allCourse.Where(i => !i.Major.Equals(userinfo.Student.Major)).Select(i => i);

            ViewModel.CourseListVM courseList = new ViewModel.CourseListVM();
            courseList.CourseList = list;
            return View(courseList);
        }
        [Authorize(Roles = "Student")]
        public ActionResult SelectedCourses()
        {
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            var list=courseBll.GetSelectedCourses(User.Identity.Name);
            double credit = 0;
            foreach (var item in list)
            {
                credit += item.Credit;
            }
            ViewModel.SelectedCoursesVM vm = new ViewModel.SelectedCoursesVM()
            {
                Credit = credit,
                SelectedCourses = list
            };
            return View(vm);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public ActionResult Select(FormCollection forms)
        {
            ViewModel.SelectCourseInfo reInfo = new ViewModel.SelectCourseInfo();

            var select = from m in forms.AllKeys
                         where forms[m].Contains("true")
                         select m;
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            List<int> hasSelected = new List<int>();
            List<int> hasFulled = new List<int>();
            List<int> hasContradic = new List<int>();
            foreach (var item in select)
            {
                if (courseBll.HasContradic(User.Identity.Name, int.Parse(item)))
                {
                    hasContradic.Add(int.Parse(item));
                    continue;
                }
                if (courseBll.HasSelected(User.Identity.Name, int.Parse(item)))
                {
                    hasSelected.Add(int.Parse(item));
                    continue;
                }
                bool re = courseBll.Select(User.Identity.Name, int.Parse(item));
                if (re != true)
                    hasFulled.Add(int.Parse(item));

            }

            var allCourse = courseBll.GetAllCourse();
            if (hasSelected.Count > 0)
            {
                reInfo.RepeatCourses = new List<string>();
                foreach (var item in hasSelected)
                {
                    var name = allCourse.Where(m => m.CourseID == item).Select(m => m.CourseName).FirstOrDefault();
                    reInfo.RepeatCourses.Add(name);
                }
            }
            if (hasFulled.Count > 0)
            {
                reInfo.FullCourses = new List<string>();
                foreach (var item in hasFulled)
                {
                    var name = allCourse.Where(m => m.CourseID == item).Select(m => m.CourseName).FirstOrDefault();
                    reInfo.FullCourses.Add(name);
                }
            }
            reInfo.AllCoursesNum = select.Count();
            reInfo.FailedNum = hasSelected.Count + hasFulled.Count + hasContradic.Count;
            reInfo.SuccessedNum = reInfo.AllCoursesNum - reInfo.FailedNum;

            var json = new JsonResult();
            json.Data = reInfo;
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        //[Authorize(Roles = "Student,Teacher")]
        //[HttpPost]
        //public ActionResult UnSelect(int? ID)
        //{
        //    Result result = new Result();
        //    BLL.CourseBLL courseBll = new BLL.CourseBLL();
        //    var re = courseBll.UnSelect(ID);
        //    if(re==true)
        //    {
        //        result.Code = 1;
        //        result.Msg = "退选成功";
        //        result.Param = ID.Value;
        //    }
        //    else
        //    {
        //        result.Code = 0;
        //        result.Msg = "退选失败";
        //    }
        //    JsonResult json = new JsonResult();
        //    json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    json.Data = result;
        //    return json;
        //}

        [Authorize(Roles = "Student,Teacher")]
        [HttpPost]
        public ActionResult StuUnSelect(int CourseID)
        {
            Result result = new Result();
            BLL.UserinfoBLL userinfoBll = new BLL.UserinfoBLL();
            var course=userinfoBll.GetUsersCourses().Where(m => m.Username == User.Identity.Name && m.CourseID == CourseID).FirstOrDefault();
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            var re = courseBll.UnSelect(User.Identity.Name, course.CourseID);
            if (re == true)
            {
                result.Code = 1;
                result.Msg = "退选成功";
                result.Param = CourseID;
            }
            else
            {
                result.Code = 0;
                result.Msg = "退选失败";
            }
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            json.Data = result;
            return json;
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        [UserInfoFilter]
        public ActionResult Schedule()
        {
            ViewModel.ViewModelBase vm = new ViewModel.ViewModelBase();
            return View(vm);
            
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public ActionResult GetSchedule()
        {
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            var list = courseBll.GetSelectedCourses(User.Identity.Name);
            //double credit = 0;
            //foreach (var item in list)
            //{
            //    credit += item.Credit;
            //}
            ViewModel.ScheduleVM vm = new ViewModel.ScheduleVM()
            {
                CourseList = list
            };
            JsonResult json = new JsonResult();
            json.Data = vm;
            json.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            return json;
        }

        [Authorize(Roles = "Student")]
        public ActionResult GetChart()
        {
            ViewModel.GetChartVM vm = new ViewModel.GetChartVM();
            BLL.UserinfoBLL userinfoBll = new BLL.UserinfoBLL();
            vm.User=userinfoBll.GetUserInfo(User.Identity.Name);
            vm.Courses=userinfoBll.GetUsersCourses().Where(m => m.Username.Equals(User.Identity.Name)).ToList();
            return View(vm);
        }
    }
}