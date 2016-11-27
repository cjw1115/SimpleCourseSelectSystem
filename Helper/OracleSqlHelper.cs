using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Runtime.Serialization;

namespace 选课系统.Helper
{
    public class OracleSqlHelper
    {
        private static string _conStr;
        static OracleSqlHelper()
        {
            var con = ConfigurationManager.ConnectionStrings["OracleConStr"];
            if (con != null)
            {
                _conStr = con.ConnectionString;
            }
        }

        /// <summary>
        /// 执行简单的增，删，改
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, params OracleParameter[] parameters )
        {
            using (OracleConnection con = new OracleConnection(_conStr))
            {
                using(OracleCommand cmd=new OracleCommand(cmdText, con))
                {
                    cmd.Parameters.AddRange(parameters);
                    try
                    {
                        con.Open();
                        var re=cmd.ExecuteNonQuery();
                        return re;
                    }
                    catch(OracleException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行一个查询的sql语句
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns>返回一个包含查询结果的datareader</returns>
        public static OracleDataReader ExecuteReader(string cmdText, params OracleParameter[] parameters)
        {
            using (var con = new OracleConnection(_conStr))
            {
                using (OracleCommand cmd = new OracleCommand(cmdText, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                    }
                    con.Open();
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        cmd.Parameters.Clear();
                        return reader;
                    }
                    catch (OracleException ex)
                    {
                        //出现异常关闭连接并且释放
                        con.Close();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一个查询的T-SQL语句, 返回一个离线数据集DataSet
        /// </summary>
        /// <param name="cmdText">要执行的T-SQL语句</param>
        /// <returns>SqlDataAdapter</returns>
        public static OracleDataAdapter ExecuteDataAdapter(string cmdText)
        {
            OracleDataAdapter adapter = new OracleDataAdapter(cmdText, _conStr);
            return adapter;
        }

        /// <summary>
        /// 返回一个离线数据集
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string cmdText, params OracleParameter[] parameters)
        {
            using (OracleDataAdapter adapter = new OracleDataAdapter(cmdText,_conStr))
            {
                using (DataSet ds = new DataSet())
                {
                    if (parameters != null)
                    {
                        adapter.SelectCommand.Parameters.Clear();
                        adapter.SelectCommand.Parameters.AddRange(parameters);
                    }
                    try
                    {
                        adapter.Fill(ds);
                        adapter.SelectCommand.Parameters.Clear();
                        return ds;
                    }
                    catch(OracleException e)
                    {
                        throw e;
                    }
                    
                }
            }
        }

        /// <summary>
        /// 将一个查询结果映射为一个clr实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="row">sql查询结果</param>
        /// <returns>最终映射的实体</returns>
        public static TEntity DataRowMapEntity<TEntity>(DataRow row) where TEntity : class, new()
        {
            try
            {
                // 获取所有属性
                var props = typeof(TEntity).GetProperties();
                // 创建返回实体
                var entity = new TEntity();
                foreach (var prop in props)
                {
                    if (prop.CanWrite)
                    {
                        try
                        {
                            if (prop.CustomAttributes.Where(m => m.AttributeType == typeof(选课系统.Attribute.IgnoreAttribute)).Count() > 0)
                                continue;
                            object data = null;
                            var attributes = prop.GetCustomAttributes(typeof(DataMemberAttribute), false);
                            if (attributes != null&& attributes.Count()>0)
                            {
                                var dataMember = attributes[0] as DataMemberAttribute;
                                data = row[dataMember.Name];
                            }
                            else
                            {
                                data = row[prop.Name];
                            }
                            
                            if (data != DBNull.Value)
                            {
                                if(data.GetType()==typeof(Decimal))
                                    if(prop.PropertyType.BaseType == typeof(Enum))
                                        prop.SetValue(entity, Enum.Parse(prop.PropertyType,data.ToString()), null);
                                    else
                                        prop.SetValue(entity , DecimalToNum(prop.PropertyType,(Decimal)data), null);
                                else
                                    prop.SetValue(entity, data,null);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            continue;
                        }
                    }
                }
                return entity;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static object DecimalToNum(Type type,decimal origin )
        {
            switch (type.Name)
            {
                case "Int32":
                    return decimal.ToInt32(origin);
                case "Double":
                    return decimal.ToDouble(origin);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 将一个SqlDataReader对象转换成一个实体类对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="reader">当前指向的reader</param>
        /// <returns>实体对象</returns>
        public static TEntity DataReaderMapEntity<TEntity>(OracleDataReader reader) where TEntity : class, new()
        {
            try
            {
                // 获取所有属性
                var props = typeof(TEntity).GetProperties();
                // 创建返回实体
                var entity = new TEntity();
                reader.Read();
                foreach (var prop in props)
                {
                    if (prop.CanWrite)
                    {
                        try
                        {
                            // 根据列名获取列在行中的序号
                            var index = reader.GetOrdinal(prop.Name);
                            // 根据序号获取当前值
                            var data = reader.GetValue(index);
                            if (data != DBNull.Value)
                            {
                                prop.SetValue(entity, Convert.ChangeType(data, prop.PropertyType), null);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            continue;
                        }
                    }
                }
                return entity;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}