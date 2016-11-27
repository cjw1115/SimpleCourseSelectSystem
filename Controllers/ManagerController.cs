using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 选课系统.Model;
using 选课系统.Filter;
namespace 选课系统.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult ShowCourses()
        {
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            var courseList=courseBll.GetAllCourse();
            ViewModel.CourseListVM vm = new ViewModel.CourseListVM();
            vm.CourseList = courseList;
            return View(vm);
        }

        private class Result
        {
            public int Code { get; set; }
            public string Msg { get; set; }
            public object Param { get; set; }
        }
        [HttpGet]
        public ActionResult AddCourse()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult AddCourse(Course course)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            Result re = new Result() { Code = 0 };
            json.Data = re;
            if (course.Mentor==null||course.CourseName==null||course.Mentor==null||course.WeekDay==null)
            {
                //必要信息不全
                re.Msg = "信息填写不完整";
                return json;
            }
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            var t = courseBll.GetCourse(course.CourseID);
            if(t!=null)
            {
                //已存在该课程号
                re.Msg = "已存在该课程号";
                return json;
            }
            if(course.EndClass<course.StartClass)
            {
                //节次错误
                re.Msg = "节次顺序错误";
                return json;
            }
            if(course.EndWeek<course.StartWeek)
            {
                //周数错误
                re.Msg = "周数顺序错误";
                return json;
            }

            courseBll.AddCourse(course);
            re.Msg = "成功添加";
            re.Code = 1;
            return json;
        }

        [HttpGet]
        public ActionResult EditCourse(int? CourseID)
        {
            if (CourseID == null)
                return RedirectToAction("ShowCourses");
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            var course=courseBll.GetCourse(CourseID.Value);
            return View(course);
        }
        [HttpPost]
        public ActionResult EditCourse(Course course)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            Result re = new Result() { Code = 0 };
            json.Data = re;
            if (course.Major == null || course.CourseName == null || course.Mentor == null || course.WeekDay == null)
            {
                //必要信息不全
                re.Msg = "信息填写不完整";
                return json;
            }
           

            if (course.EndClass < course.StartClass)
            {
                //节次错误
                re.Msg = "节次顺序错误";
                return json;
            }
            if (course.EndWeek < course.StartWeek)
            {
                //周数错误
                re.Msg = "周数顺序错误";
                return json;
            }
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            courseBll.ChangeCourse(course);
            re.Msg = "修改成功";
            re.Code = 1;
            return json;
        }

        public ActionResult DeleteCourse(int? CourseID)
        {
            Result result = new Result() { Code = 0, Msg = "参数异常" };
            JsonResult json = new JsonResult();
            json.Data = result;
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (CourseID == null)
                return json;

            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            bool re=courseBll.DeleteCourse(CourseID.Value);
           
            if(re==true)
            {
                result.Code = 1;
                result.Msg = "删除成功！";
                result.Param = CourseID;
            }
            else
            {
                result.Code = 0;
                result.Msg = "删除失败";

            }
           
            return json;
        }

        public ActionResult SetFlag()
        {
            Result result = new Result();
            result.Code = 1;
            result.Msg = "设置课标成功！";

            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            courseBll.SetFlag();

            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            json.Data = result;

            return json;
        }

        [HttpGet]
        public ActionResult ShowUsersCourses()
        {
            ViewModel.ShowUsersCoursesVM vm = new ViewModel.ShowUsersCoursesVM() { Param = new SelectedCourse() };
            BLL.UserinfoBLL userinfoBll = new BLL.UserinfoBLL();
            var list=userinfoBll.GetUsersCourses();
            vm.SelectedCourses = list;
            return View(vm);
        }
        
        [ExcelFileFilter]
        public ActionResult GetUsersCoursesExcel()
        {
            ViewModel.ShowUsersCoursesVM vm = new ViewModel.ShowUsersCoursesVM() { Param = new SelectedCourse() };
            BLL.UserinfoBLL userinfoBll = new BLL.UserinfoBLL();
            var list = userinfoBll.GetUsersCourses();
            vm.SelectedCourses = list;
            //Response.ContentType = "application/ms-excel";
            return View("ShowUsersCoursesPartialForExcel", vm);
        }
        [HttpPost]
        public ActionResult ShowUsersCourses(SelectedCourse selectedCourse)
        {
            
            BLL.UserinfoBLL userinfoBll = new BLL.UserinfoBLL();
            var list = userinfoBll.GetUsersCourses();
            IEnumerable<SelectedCourse>  reList = list.ToList();
            if (!string.IsNullOrEmpty(selectedCourse.Username))
            {
                reList = list.Where(m => m.Username.Contains(selectedCourse.Username));
            }
            if (!string.IsNullOrEmpty(selectedCourse.Name))
            {
                reList = reList.Where(m => m.Name.Contains(selectedCourse.Name));
            }
            if (!string.IsNullOrEmpty(selectedCourse.CourseName))
            {
                reList = reList.Where(m => m.CourseName.Contains(selectedCourse.CourseName));
            }
            if (!string.IsNullOrEmpty(selectedCourse.Mentor))
            {
                reList = reList.Where(m => m.Mentor.Contains(selectedCourse.Mentor));
            }
            if(selectedCourse.CourseID!=0)
            {
                reList = reList.Where(m => m.CourseID== selectedCourse.CourseID);
            }
           
            ViewModel.ShowUsersCoursesVM vm = new ViewModel.ShowUsersCoursesVM();
            vm.SelectedCourses = reList.ToList();
            vm.Param = selectedCourse;
            return View("ShowUsersCoursesPartial",vm);
        }

        [HttpGet]
        public ActionResult ShowCoursesInfo()
        {
            ViewModel.ShowCoursesInfoVM vm = new ViewModel.ShowCoursesInfoVM();
            BLL.CourseBLL courseBll = new BLL.CourseBLL();
            var list = courseBll.GetAllCourse().Where(m => m.RealNum > 0);
            vm.Couses = list;
            return View(vm);
        }
        [HttpPost]
        public ActionResult ShowCoursesInfo(int CourseID )
        {
            BLL.UserinfoBLL userinfoBll = new BLL.UserinfoBLL();
            var list = userinfoBll.GetUsersCourses();
            IEnumerable<SelectedCourse> reList = list.ToList();
            
            if (CourseID!= 0)
            {
                reList = reList.Where(m => m.CourseID == CourseID);
            }

            ViewModel.ShowUsersCoursesVM vm = new ViewModel.ShowUsersCoursesVM();
            vm.SelectedCourses = reList.ToList();
            return View("ShowCoursesInfoPartial", vm);
        }

        [ExcelFileFilter]
        public ActionResult GetCoursesInfoExcel(int? CourseID)
        {
            if (CourseID!=null&&CourseID.Value!= 0)
            {
                ViewModel.ShowUsersCoursesVM vm = new ViewModel.ShowUsersCoursesVM() { Param = new SelectedCourse() };
                BLL.UserinfoBLL userinfoBll = new BLL.UserinfoBLL();
                var list = userinfoBll.GetUsersCourses().Where(m => m.CourseID == CourseID.Value);
                vm.SelectedCourses = list;
                //Response.ContentType = "application/ms-excel";
                return View("ShowUsersCoursesPartialForExcel", vm);
            }
            else
            {
                ViewModel.ShowCoursesInfoVM vm = new ViewModel.ShowCoursesInfoVM();
                BLL.CourseBLL courseBll = new BLL.CourseBLL();
                var list = courseBll.GetAllCourse().Where(m => m.RealNum > 0);
                vm.Couses = list;
                return View("ShowCoursesInfoExcel", vm);
            }
            
        }
    }
    
}