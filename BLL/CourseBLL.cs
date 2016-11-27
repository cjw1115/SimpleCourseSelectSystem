using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 选课系统.Model;
using Oracle.ManagedDataAccess.Client;
using 选课系统.Helper;

namespace 选课系统.BLL
{
    public class CourseBLL
    {
        private DAL.CourseDAL courseDAL;
        public CourseBLL()
        {
            courseDAL = new DAL.CourseDAL();
        }
        //获取所有课程信息
        public List<Course> GetAllCourse()
        {
            return courseDAL.GetAllCourse();
        }
        public Course GetCourse(int courseID)
        {
            return courseDAL.GetCourse(courseID);
        }

        public bool AddCourse(Course course)
        {
            return courseDAL.AddCourse(course);
        }

        public bool ChangeCourse(Course course)
        {
            return courseDAL.ChangeCourse(course);
        }


        public bool DeleteCourse(int courseID)
        {
            return courseDAL.DeleteCourse(courseID);
        }
        public IList<Course> GetSelectedCourses(string username)
        {
            var courseList = new List<Course>();

            String cmdStr = "select * from \"TCourse\" WHERE \"ID\" IN(select \"CourseID\" from \"TSelectedCourse\" WHERE \"StudentID\" = (select \"ID\" from \"TStudent\" where \"UserID\" = :Username AND ROWNUM <= 1))";
            var dataSet = OracleSqlHelper.ExecuteDataSet(cmdStr, new OracleParameter(":Username", username));
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows!=null)
            {
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    var course=OracleSqlHelper.DataRowMapEntity<Course>(item);
                    courseList.Add(course);
                }
            }
            return courseList;
        }
            //选择一门课程
        public bool Select(string username,int courseID)
        {
            return courseDAL.Select(username, courseID);
        }
        public bool UnSelect(string username, int courseID)
        {
            return courseDAL.UnSelect(username,courseID);
        }

        public bool HasSelected(string username,int courseID)
        {
            return courseDAL.HasSelected(username, courseID);
        }

        public bool HasContradic(string username, int courseID)
        {
            string studentIDCmdStr = "select \"ID\" from \"TStudent\" where \"UserID\"=:Username and rownum<=1";
            var dataSet = OracleSqlHelper.ExecuteDataSet(studentIDCmdStr, new OracleParameter(":Username", username));
            int studentID=decimal.ToInt32((decimal)dataSet.Tables[0].Rows[0][0]);
          
            string isContradicCmdStr= "select * from \"TCourse\" WHERE \"ID\" IN (select \"CourseID\" from \"TSelectedCourse\" WHERE \"StudentID\"=:StudentID) and \"Flag\" = (select \"Flag\" from \"TCourse\" WHERE \"ID\" = :CourseID) ";

           
            dataSet = OracleSqlHelper.ExecuteDataSet(isContradicCmdStr, new OracleParameter(":StudentID", studentID),new OracleParameter(":CourseID",courseID));
            if (dataSet!=null&&dataSet.Tables.Count>0&&dataSet.Tables[0].Rows.Count>0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 设置 课程标志，防止选课冲突
        /// </summary>
        /// <returns>void</returns>
        public void SetFlag()
        {
            string sqlReset = "update \"TCourse\" set \"Flag\"=0";
            OracleSqlHelper.ExecuteNonQuery(sqlReset);

            string cmdStr = "select * from \"TCourse\"";
            using (OracleDataAdapter adpter = OracleSqlHelper.ExecuteDataAdapter(cmdStr))
            {
                DataSet ds = new DataSet();
                adpter.Fill(ds);
                if (ds.Tables.Count >= 0 && ds.Tables[0] != null)
                {
                    int flag = 1;
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        if (row["Flag"] != DBNull.Value && int.Parse(row["Flag"].ToString()) > 0)//已被标记
                        {
                            //
                        }
                        else//未被标记
                        {
                            row["Flag"] = flag;//做标记
                                               //标记完后检测和他重复的，遇到重复的，就将其也标记
                            int StartClass = int.Parse(row["StartClass"].ToString());
                            int EndClass = int.Parse(row["EndClass"].ToString());
                            int StartWeek = int.Parse(row["StartWeek"].ToString());
                            int EndWeek = int.Parse(row["EndWeek"].ToString());
                            int weekday = ParseWeekDay(row["WeekDay"].ToString());


                            IsRepeat(dt, i, flag, StartClass, EndClass, StartWeek, EndWeek, weekday);

                            flag++;
                        }

                    }
                }
                using (OracleCommandBuilder cb = new OracleCommandBuilder(adpter))
                {
                    cb.DataAdapter.Update(ds);
                }
            }
                
        }
        public bool IsRepeat(DataTable dt, int current, int flag, int StartClass, int EndClass, int sweek, int eweek, int weekday)
        {
            bool re = false;
            for (int i = current + 1; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                //已经标记过的不再标记
                if (row["Flag"] != DBNull.Value && int.Parse(row["Flag"].ToString()) > 0)
                    continue;
                int zhoushu1 = int.Parse(row["StartWeek"].ToString());
                int zhoushu2 = int.Parse(row["EndWeek"].ToString());
                int jieci1 = int.Parse(row["StartClass"].ToString());
                int jieci2 = int.Parse(row["EndClass"].ToString());
                int day = ParseWeekDay(row["WeekDay"].ToString());
                if (IsContains(sweek, eweek, zhoushu1, zhoushu2))
                {
                    if (weekday == day)
                    {
                        if (IsContains(StartClass, EndClass, jieci1, jieci2))
                        {
                            row["Flag"] = flag;//标记冲突的
                            re = true;
                        }
                    }
                }
            }
            return re;

        }

        private bool IsContains(int s1, int e1, int s2, int e2)
        {
            int dis1 = e1 - s1;
            int dis2 = e2 - s2;
            if (dis2 < dis1)
            {
                int t = e1;
                e1 = e2;
                e2 = t;

                t = s1;
                s1 = s2;
                s2 = t;
            }
            if ((s1 >= s2 && s1 <= e2) || (e1 >= s2 && e1 <= e2))
            {
                return true;
            }
            else
                return false;
        }
        private int ParseWeekDay(string day)
        {
            switch (day.Trim())
            {
                case "周一":
                    return 1;
                case "周二":
                    return 2;
                case "周三":
                    return 3;
                case "周四":
                    return 4;
                case "周五":
                    return 5;
                case "周六":
                    return 6;
                case "周日":
                    return 7;
            }
            return 0;
        }
    }
}
