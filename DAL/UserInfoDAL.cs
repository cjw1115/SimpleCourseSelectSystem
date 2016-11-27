using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 选课系统.Model;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using 选课系统.Helper;
using System.Data;

namespace 选课系统.DAL
{
    public class UserInfoDAL
    {
        public User GetUserInfo(string username)
        {
            string cmdStr = "select * from \"TUser\" where \"Username\"=:Username";
            var dataSet = OracleSqlHelper.ExecuteDataSet(cmdStr, new OracleParameter(":Username", username));
            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                var user = OracleSqlHelper.DataRowMapEntity<User>(row);
                if (user.UserType == UserType.Student)
                {
                    cmdStr = "select * from \"TStudent\" where \"UserID\"=:Username and rownum<=1";
                    dataSet = OracleSqlHelper.ExecuteDataSet(cmdStr, new OracleParameter(":Username", username));
                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        row = dataSet.Tables[0].Rows[0];
                        var student = OracleSqlHelper.DataRowMapEntity<Student>(row);
                        user.Student = student;
                    }
                }
                return user;
            }
            return null;
        }
        
        public IList<SelectedCourse> GetUsersCourses()
        {
            List<SelectedCourse> selectedCourses = new List<SelectedCourse>();
            string scCmdStr= "select DISTINCT \"TUser\".\"Username\",\"TUser\".\"Name\",\"TCourse\".\"CourseName\",\"TCourse\".\"Mentor\",\"TCourse\".\"Credit\",\"TSelectedCourse\".\"CourseID\",\"TSelectedCourse\".\"StudentID\",\"TSelectedCourse\".\"Score\" from \"TSelectedCourse\",\"TCourse\",\"TStudent\",\"TUser\" WHERE  \"TSelectedCourse\".\"CourseID\" = \"TCourse\".\"ID\" and \"TUser\".\"Username\" = (select \"UserID\" from \"TStudent\" where \"TSelectedCourse\".\"StudentID\" = \"TStudent\".\"ID\" and ROWNUM<= 1)";

            var dataSet=OracleSqlHelper.ExecuteDataSet(scCmdStr);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    var selectedCourse=OracleSqlHelper.DataRowMapEntity<SelectedCourse>(item);
                    selectedCourses.Add(selectedCourse);
                }
            }
            return selectedCourses;
        }
        public bool AddUser(User user)
        {
            List< OracleParameter> userParams = null;
            List<OracleParameter> studentParams = null;
            string insertUser = "insert into \"TUser\"(\"Username\",\"Password\",\"Name\",\"Gender\",\"UserType\",\"Contact\") values(:Username,:Password,:Name,:Gender,:UserType,:Contact)";

            string insertStudent = "insert into \"TStudent\"(\"ID\",\"Mentor\",\"Major\",\"PersonInfo\",\"UserID\") values(AutoStudentID.nextval,:Mentor,:Major,:PersonInfo,:UserID)";
            userParams = new List<OracleParameter>();
            userParams.Add(new OracleParameter(":Username", user.Username));
            userParams.Add(new OracleParameter(":Password", user.Password));
            userParams.Add(new OracleParameter(":Name", user.Name));
            userParams.Add(new OracleParameter(":Gender", user.Gender));
            userParams.Add(new OracleParameter(":UserType", (int)user.UserType));
            userParams.Add(new OracleParameter(":Contact", user.Contact));

            if (user.UserType == UserType.Student && user.Student != null)
            {
                studentParams = new List<OracleParameter>();
                studentParams.Add(new OracleParameter(":Mentor", user.Student.Mentor));
                studentParams.Add(new OracleParameter(":Major", user.Student.Major));
                studentParams.Add(new OracleParameter(":PersonInfo", user.Student.PersonInfo));
                studentParams.Add(new OracleParameter(":UserID", user.Username));
            }

            var re = OracleSqlHelper.ExecuteNonQuery(insertUser, userParams.ToArray());
            if (re > 0)
            {
                if (user.UserType == UserType.Student && user.Student != null)
                {
                    re = OracleSqlHelper.ExecuteNonQuery(insertStudent, studentParams.ToArray());
                    if (re > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
                
            }
            else
                return false;
                  
        }
        public bool UpdateUser(User user)
        {
            if (!HasExist(user.Username))
                return false;

            OracleParameter[] userParams = null;
            OracleParameter[] studentParams = null;

            string userCmdStr = "update (select * from \"TUser\" where \"TUser\".\"Username\"=:Username ) set \"Name\"=:Name,\"Gender\"=:Gender,\"Password\"=:Password,\"UserType\"=:UserType,\"Contact\"=:Contact";
            string stuCmdSTR= "update (select * from \"TStudent\" where \"ID\"=(select \"ID\" from \"TStudent\" where \"UserID\"=:Username and ROWNUM<=1)) set \"Mentor\"=:Mentor,\"Major\"=:Major,\"PersonInfo\"=:PersonInfo";

            userParams = new OracleParameter[]
            {
                new OracleParameter(":Username",user.Username),
                new OracleParameter(":Name",user.Name),
                new OracleParameter(":Gender",user.Gender),
                new OracleParameter(":Password",user.Password),
                new OracleParameter(":UserType",(int)user.UserType),
                new OracleParameter(":Contact",user.Contact)
                
            };
            if (user.UserType == UserType.Student && user.Student != null)
            {
                studentParams = new OracleParameter[]
                {
                    new OracleParameter(":Username",user.Username),
                    new OracleParameter(":Mentor",user.Student.Mentor),
                    new OracleParameter(":Major",user.Student.Major),
                    new OracleParameter(":PersonInfo",user.Student.PersonInfo)
                };
            }
            var re = OracleSqlHelper.ExecuteNonQuery(userCmdStr, userParams);
            if (re > 0)
            {
                if (user.UserType == UserType.Student && user.Student != null)
                {
                    re = OracleSqlHelper.ExecuteNonQuery(stuCmdSTR, studentParams);
                    if (re > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return true;

            }
            else
                return false;
        }
        public bool HasExist(string username)
        {
            string cmdStr = "select * from \"TUser\" where \"Username\"=:Username";
            var dataSet=OracleSqlHelper.ExecuteDataSet(cmdStr, new
                OracleParameter("Username", username));
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
                return false;
        }

    }
}
