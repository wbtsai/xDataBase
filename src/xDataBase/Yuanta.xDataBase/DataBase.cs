using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace Yuanta.xDataBase
{
    public class DataBase
    {
        private string Name { get; set; }
        public DbProviderFactory ProveiderFactory { get; set; }
        public DbConnection Connection { get; set; }
        public String ParameterToken { get; set; }
                
        public DataBase(string name)
        {
            this.Name = name;

            Create();
        }

        private void Create()
        {
            ConnectionStringSettings connStringSetting = ConfigurationManager.ConnectionStrings[this.Name];

            this.ProveiderFactory = DbProviderFactories.GetFactory(connStringSetting.ProviderName);

            DbConnection newConnection=this.ProveiderFactory.CreateConnection();

            newConnection.ConnectionString = connStringSetting.ConnectionString;

            this.Connection=newConnection;

            switch (connStringSetting.ProviderName)
            {
                case "System.Data.SqlClient":
                    this.ParameterToken = "@";
                    break;
                case "Oracle.DataAccess.Client":
                    this.ParameterToken = ":";
                    break;
                default:
                    throw new Exception("尚末實作此項目");
            }
        }

        private DbCommand CreateCommand()
        {
            return this.Connection.CreateCommand();
        }

        private DbCommand CreateCommand(string sql,CommandType cmdType,Action<Dictionary<string, object>> parameterList)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            DbCommand cmd = CreateCommand();

            cmd.CommandText = sql;

            parameterList(dic);

            List<string> pList = GetParameterList(sql, this.ParameterToken);

            if (pList.Count > 0)
            {
                if (dic != null)
                {
                    foreach (string p in pList)
                    {
                        if (dic[p].GetType() ==this.ProveiderFactory.CreateParameter().GetType())
                        {
                            cmd.Parameters.Add(dic[p]);
                        }
                        else
                        {
                            DbParameter pp = this.ProveiderFactory.CreateParameter();
                            pp.ParameterName = p;
                            pp.Value = dic[p];

                            cmd.Parameters.Add(pp);
                        }
                    }
                }
            }
            else
            {
                if (cmdType == CommandType.StoredProcedure)
                {
                    if (dic.Count > 0)
                    {
                        foreach (string p in dic.Keys)
                        {
                            if (dic[p].GetType() == this.ProveiderFactory.CreateParameter().GetType())
                            {
                                cmd.Parameters.Add(dic[p]);
                            }
                            else
                            {
                                DbParameter pp = this.ProveiderFactory.CreateParameter();
                                pp.ParameterName = p;
                                pp.Value = dic[p];

                                cmd.Parameters.Add(pp);
                            }
                        }
                    }
                }
            }
            return cmd;
        }

        public DataTable DoQuery(Func<string> sqlStatement, Action<Dictionary<string, object>> AddParamters)
        {
            return DoQuery(sqlStatement,CommandType.Text,AddParamters);
        }
        public DataTable DoQuery(Func<string> sqlStatement,CommandType cmdType, Action<Dictionary<string, object>> AddParamters)
        {
            DbCommand cmd = this.CreateCommand(sqlStatement(), cmdType, AddParamters);

            return this.LoadDataTable(cmd);
        }

        private DbParameter CreateParameter()
        {
            return this.ProveiderFactory.CreateParameter();
        }

        private DataTable LoadDataTable(DbCommand cmd)
        {
            DataTable dt = new DataTable();
            cmd.Connection = this.Connection;

            using (DbDataAdapter adapter =this.ProveiderFactory.CreateDataAdapter())
            {
                ((IDbDataAdapter)adapter).SelectCommand = cmd;

                adapter.Fill(dt);
            }
            return dt;
        }

        public int DoCommand(Func<string> sqlStatement,Action<Dictionary<string, object>> AddParamters)
        {
           return DoCommand(sqlStatement, CommandType.Text,AddParamters);
        }

        public int DoSPCommand(Func<string> sqlStatement,Action<Dictionary<string, object>> AddParamters)
        {
           return DoCommand(sqlStatement, CommandType.StoredProcedure,AddParamters);
        }

        public int DoCommand(Func<string> sqlStatement, CommandType commandType, Action<Dictionary<string, object>> AddParamters)
        {
            DbCommand cmd;

            try
            {
                cmd = this.CreateCommand(sqlStatement(), commandType, AddParamters);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected;
            }
            catch (Exception ex)
            {
                string err = string.Empty;

                if (ex.InnerException != null)
                {
                    err = ex.InnerException.Message;
                }
                throw new Exception(ex.Message + err);
            }
        }

        public void DoTransaction(bool isTransction, Action myCommandExec)
        {
            using (DbConnection conn = this.Connection)
            {
                conn.Open();

                if (isTransction)
                {
                    using (DbTransaction tran = conn.BeginTransaction())
                    {
                        //執行 Command
                        myCommandExec();

                        tran.Commit();
                    }
                }
                else
                {
                    myCommandExec();
                }
            }
        }

        private  List<string> GetParameterList(string sql, string ParameterToken)
        {
            List<string> list = new List<string>();

            Regex regex = new Regex(ParameterToken + "(?<name>(\\w+))");

            MatchCollection matches = regex.Matches(sql);

            foreach (Match match in matches)
            {
                list.Add(match.Groups["name"].Value);
            }

            return list;
        }

        private void RowMapping()
        { 
            
        }
    }
}
