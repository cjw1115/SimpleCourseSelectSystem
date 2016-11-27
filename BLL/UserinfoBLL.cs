using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 选课系统.Model;

namespace 选课系统.BLL
{
    public class UserinfoBLL
    {
        public User GetUserInfo(string username)
        {
            DAL.UserInfoDAL userinfoDal = new DAL.UserInfoDAL();
            User userinfo=userinfoDal.GetUserInfo(username);
            return userinfo;
        }
        public IList<SelectedCourse> GetUsersCourses()
        {
            DAL.UserInfoDAL userinfoDal = new DAL.UserInfoDAL();
            return userinfoDal.GetUsersCourses();
        }
        public bool Regist(User userinfo)
        {
            
            DAL.UserInfoDAL userinfoDal = new DAL.UserInfoDAL();

            //UserInfo userinfo = new UserInfo() { Username = username, Password = password ,UserType= userType};
            //if (studentInfo != null)
            //{
            //    studentInfo.Userinfo = userinfo;
            //}
            //userinfo.Studentinfo = studentInfo;

            return userinfoDal.AddUser(userinfo);
        }
        public bool Change(User userinfo)
        {
            DAL.UserInfoDAL userinfoDal = new DAL.UserInfoDAL();
           // UserInfo userinfo = new UserInfo() { Username = username, Password = newPassword, UserType = userType};
            //if (studentInfo != null)
            //{
            //    studentInfo.Userinfo = userinfo;
            //}
            //userinfo.Studentinfo = studentInfo;

            return userinfoDal.UpdateUser(userinfo);
        }
        public bool HasExist(string username)
        {
            DAL.UserInfoDAL userinfoDal = new DAL.UserInfoDAL();
            
            return userinfoDal.HasExist(username);
        }

        public static String[] GetUserRoles(string username)
        {
            DAL.UserInfoDAL userinfoDal = new DAL.UserInfoDAL();
            var userinfo= userinfoDal.GetUserInfo(username);
            return new[] { userinfo.UserType.ToString() };
        }
    }
}
