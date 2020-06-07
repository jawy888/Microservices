using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Web.Configs;
using TaskScheduler.Web.Models;

namespace TaskScheduler.Web.Infrastructure.Repository
{
    public class TaskRepository
    {
        public static bool DeleteEnv(string id)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "delete from env where id=@id";
                var result = conn.Execute(sql, new { id});
                return result>0;
            }
        }

        public static bool UpdateEnv(EnvOptions env)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "update env set `key`=@key,val=@val where id=@id";
                var result = conn.Execute(sql, env);
                return result > 0;
            }
        }

        public static List<EnvOptions> GetEnv()
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "select * from env order by create_time desc";
                var result = conn.Query<EnvOptions>(sql);
                return result.ToList();
            }
        }

        public static EnvOptions GetEnvByKey(string key)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "select * from env where `key`=@key";
                var result = conn.QueryFirstOrDefault<EnvOptions>(sql,new { key});
                return result;
            }
        }
        public static bool AddEnv(EnvOptions env)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                env.id = Guid.NewGuid().ToString("N");
                env.create_time = DateTime.Now;
                var sql = "insert into env(id,`key`,val,create_time) values(@id,@key,@val,@create_time)";
                var result = conn.Execute(sql, env);
                return result > 0;
            }
        }

        public static bool AddTask(TaskOptions taskOptions)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                taskOptions.id = Guid.NewGuid().ToString("N");
                taskOptions.create_time = DateTime.Now;
                var sql = "insert into task(id,`name`,group_name,`interval`,api_url,auth_key,auth_value,`describe`,request_method,last_time,create_time,`status`) values(@id,@name,@group_name,@interval,@api_url,@auth_key,@auth_value,@describe,@request_method,@last_time,@create_time,@status)";
                var result = conn.Execute(sql, taskOptions);
                return result > 0;
            }
        }
        public static bool UpdateTask(TaskOptions taskOptions)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "update task set `name`=@name,group_name=@group_name,`interval`=@interval,api_url=@api_url,auth_key=@auth_key,auth_value=@auth_value,`describe`=@describe,request_method=@request_method,last_time=@last_time,`status`=@status where id=@id";
                var result = conn.Execute(sql, taskOptions);
                return result > 0;
            }
        }
        public static bool DeleteTask(IEnumerable<string> ids)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "delete from task where id in @ids";
                var r1 = conn.Execute(sql, new { ids });
                var sqlLog = "delete from task_log where task_id in @ids";
                var r2 = conn.Execute(sqlLog, new { ids });
                if (r1 > 0 && r2 > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public static List<TaskOptions> GetTaskList(string name = "", string groupName = "")
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "select * from task where 1=1";
                if (!string.IsNullOrEmpty(name))
                {
                    sql += " and name like @name";
                }
                if (!string.IsNullOrEmpty(groupName))
                {
                    sql += " and group_name=@groupName";
                }
                sql += " order by create_time desc";
                var result = conn.Query<TaskOptions>(sql, new { name=$"%{name}%", groupName });
                return result.ToList();
            }
        }

        public static bool AddTaskLog(TaskLog taskLog)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "insert into task_log(id,task_id,response,execute_time,apm) values(@id,@task_id,@response,@execute_time,@apm)";
                var result = conn.Execute(sql, taskLog);
                return result > 0;
            }
        }

        public static List<TaskLog> GetTaskLog(string id)
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "select * from task_log where task_id=@id order by execute_time desc limit 15";
                var result = conn.Query<TaskLog>(sql, new { id });
                return result.ToList();
            }
        }

        public static List<string> GetTaskGroupName()
        {
            using (var conn = new MySqlConnection(Global.MySQLConnection))
            {
                var sql = "select group_name from task group by group_name";
                var result = conn.Query<string>(sql);
                return result.ToList();
            }
        }
    }
}
