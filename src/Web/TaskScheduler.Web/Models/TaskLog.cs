using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskScheduler.Web.Models
{
    /// <summary>
    /// 任务执行日志
    /// </summary>
    public class TaskLog
    {
        public string id { get; set; }
        public string task_id { get; set; }
        public DateTime execute_time { get; set; }
        public long apm { get; set; }
        public string response { get; set; }
    }
}
