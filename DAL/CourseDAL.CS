using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 选课系统.Helper;
using 选课系统.Model;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;

namespace 选课系统.DAL
{
    public class CourseDAL
    {
        public List<Course> GetAllCourse()
        {
            string sql = "select * from \"TCourse\"";
            DataSet dataSet = OracleSqlHelper.ExecuteDataSet(sql);
            DataTable table = dataSet.Tables[0];
            List<Course> courseList = new List<Course>();

            foreach (DataRow item in table.Rows)
            {
                Course course = OracleSqlHelper.DataRowMapEntity<Course>(item);
                courseList.Add(course);
            }
            return courseList;
        }
        public bool AddCourse(Course course)
        {
            string insertCmdStr = "insert into \"TCourse\"(\"ID\",\"Major\", \"CourseName\", \"Mentor\", \"Credit\", \"StartClass\", \"EndClass\",\"WeekDay\", \"StartWeek\", \"EndWeek\", \"AllWeek\", \"MaxNum\", \"RealNum\", \"Room\") VALUES(AutoCourseID.nextval,:Major, :CourseName, :Mentor, :Credit, :StartClass, :EndClass,:WeekDay, :StartWeek, :EndWeek, :AllWeek, :MaxNum, :RealNum,:Room)";
            List<OracleParameter> parms = new List<OracleParameter>();
            parms.Add(new OracleParameter(":Major", course.Major));
            parms.Add(new OracleParameter(":CourseName", course.CourseName));
            parms.Add(new OracleParameter(":Mentor", course.Mentor));
            parms.Add(new OracleParameter(":Credit", course.Credit));
            parms.Add(new OracleParameter(":StartClass", course.StartClass));
            parms.Add(new OracleParameter(":EndClass", course.EndClass));
            parms.Add(new OracleParameter(":WeekDay", course.WeekDay));
            parms.Add(new OracleParameter(":StartWeek", course.StartWeek));
            parms.Add(new OracleParameter(":EndWeek", course.EndWeek));
            parms.Add(new OracleParameter(":AllWeek", course.AllWeek));
            parms.Add(new OracleParameter(":MaxNum", course.MaxNum));
            parms.Add(new OracleParameter(":RealNum", course.RealNum));
            parms.Add(new OracleParameter(":Room", course.Room));

            int re=OracleSqlHelper.ExecuteNonQuery(insertCmdStr, parms.ToArray());
            if (re > 0)
                return true;
            else
                return false;
        }
        public bool ChangeCourse(Course course)
        {
            string courseCmdStr = "update (select * from \"TCourse\" where \"ID\"=:CourseID) set \"Major\"=:Major, \"CourseName\"=:CourseName,\"Mentor\"=:Mentor, \"Credit\"=:Credit, \"StartClass\"=:StartClass, \"EndClass\"=:EndClass,\"WeekDay\"=:WeekDay, \"StartWeek\"=:StartWeek, \"EndWeek\"=:EndWeek, \"AllWeek\"=:AllWeek, \"MaxNum\"=:MaxNum, \"RealNum\"=:RealNum, \"Room\"=:Room";
            List<OracleParameter> parms = new List<OracleParameter>();
            parms.Add(new OracleParameter(":CourseID", course.CourseID));
            parms.Add(new OracleParameter(":Major", course.Major));
            parms.Add(new OracleParameter(":CourseName", course.CourseName));
            parms.Add(new OracleParameter(":Mentor", course.Mentor));
            parms.Add(new OracleParameter(":Credit", course.Credit));
            parms.Add(new OracleParameter(":StartClass", course.StartClass));
            parms.Add(new OracleParameter(":EndClass", course.EndClass));
            parms.Add(new OracleParameter(":WeekDay", course.WeekDay));
            parms.Add(new OracleParameter(":StartWeek", course.StartWeek));
            parms.Add(new OracleParameter(":EndWeek", course.EndWeek));
            parms.Add(new OracleParameter(":AllWeek", course.AllWeek));
            parms.Add(new OracleParameter(":MaxNum", course.MaxNum));
            parms.Add(new OracleParameter(":RealNum", course.RealNum));
            parms.Add(new OracleParameter(":Room", course.Room));
            int re = OracleSqlHelper.ExecuteNonQuery(courseCmdStr, parms.ToArray());
            if (re > 0)
                return true;
            else
                return false;
        }
        public bool DeleteCourse(int courseID)
        {
            string cmdStr = "delete from  \"TCourse\" where \"ID\"=:CourseID";

            int re = OracleSqlHelper.ExecuteNonQuery(cmdStr, new OracleParameter(":CourseID", courseID));
            if (re > 0)
                return true;
            else
                return false;
        }
        public Course GetCourse(int CourseID)
        {
            Course course = null;
            string cmdStr = "select * from \"TCourse\" where \"ID\"=:CourseID";
            using (var dataSet = OracleSqlHelper.ExecuteDataSet(cmdStr, new OracleParameter(":CourseID", CourseID)))
            {
                if (dataSet != null && dataSet.Tables.Count > 0&&dataSet.Tables[0].Rows.Count>0)
                {
                    course = OracleSqlHelper.DataRowMapEntity<Course>(dataSet.Tables[0].Rows[0]);
                }
                
            }
            return course;   
        }
        public bool Select(string username,int courseID)
        {
            string cmdStr = "select \"MaxNum\",\"RealNum\" from \"TCourse\" WHERE \"ID\"=:CourseID";
            var dataSet = OracleSqlHelper.ExecuteDataSet(cmdStr, new OracleParameter(":CourseID", courseID));

            int max =decimal.ToInt32((decimal)dataSet.Tables[0].Rows[0][0]);
            int real = decimal.ToInt32((decimal)dataSet.Tables[0].Rows[0][1]);
            //int flag = reader.GetInt32(2);
            if (real + 1 <= max)
            {
                string realnumCmdStr = "update \"TCourse\" SET \"RealNum\"=\"RealNum\"+1 where \"ID\"=:CourseID";
                if (OracleSqlHelper.ExecuteNonQuery(realnumCmdStr,new OracleParameter(":CourseID", courseID)) >= 1)
                {
                    string studentIDCmdStr = "select \"ID\" from \"TStudent\" where \"UserID\"=:Username and rownum<=1";
                    var re = OracleSqlHelper.ExecuteDataSet(studentIDCmdStr, new OracleParameter(":Username", username));
                    int studentID = decimal.ToInt32((decimal)re.Tables[0].Rows[0][0]);

                    string scCmdStr= "insert into \"TSelectedCourse\"(\"CourseID\",\"StudentID\") VALUES(:CourseID,:StudentID)";
                    if(OracleSqlHelper.ExecuteNonQuery(scCmdStr, new OracleParameter(":CousreID", courseID), new OracleParameter(":StudentID", studentID))>0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
        public bool UnSelect(string username,int courseID)
        {
            string cmdStr = "select \"MaxNum\",\"RealNum\" from \"TCourse\" WHERE \"ID\"=:CourseID";
            var dataSet = OracleSqlHelper.ExecuteDataSet(cmdStr, new OracleParameter(":CourseID", courseID));
            int max = decimal.ToInt32((decimal)dataSet.Tables[0].Rows[0][0]);
            int real = decimal.ToInt32((decimal)dataSet.Tables[0].Rows[0][1]);
            //int flag = reader.GetInt32(2);
            if (real - 1 >=0)
            {
                string realnumCmdStr = "update \"TCourse\" SET \"RealNum\"=\"RealNum\"-1 where \"ID\"=:CourseID";
                if (OracleSqlHelper.ExecuteNonQuery(realnumCmdStr, new OracleParameter(":CousreID", courseID)) >= 1)
                {
                    string studentIDCmdStr="select \"ID\" from \"TStudent\" where \"UserID\"=:Username and rownum<=1";
                    var re= OracleSqlHelper.ExecuteDataSet(studentIDCmdStr,new OracleParameter(":Username",username));
                    
                    int studentID= decimal.ToInt32((decimal)re.Tables[0].Rows[0][0]);
                    string scCmdStr = "delete from \"TSelectedCourse\" where \"StudentID\"=:StudentID and \"CourseID\"=:CourseID";
                    if( OracleSqlHelper.ExecuteNonQuery(scCmdStr, new OracleParameter(":StudentID", studentID), new OracleParameter(":CousreID", courseID)) >0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
        public bool HasSelected(string username, int courseID)
        {
            string cmdStr= "select * from \"TSelectedCourse\" WHERE \"StudentID\"=(select  \"ID\" from \"TStudent\" where \"UserID\"=:Username and rownum<=1) and \"CourseID\"=:CourseID";
            OracleParameter[] param = new OracleParameter[]
            {
                new OracleParameter(":Username",username),
                new OracleParameter(":CourseID",courseID)
            };
            var dataSet=OracleSqlHelper.ExecuteDataSet(cmdStr, param);

            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
        
    }
}
