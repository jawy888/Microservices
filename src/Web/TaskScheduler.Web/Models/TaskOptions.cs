using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskScheduler.Web.Models
{
    public class TaskOptions
    {
        public string id { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string group_name { get; set; }
        /// <summary>
        /// 间隔
        /// </summary>
        public string interval { get; set; }
        /// <summary>
        /// api地址
        /// </summary>
        public string api_url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string auth_key { get; set; }
        public string auth_value { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 请求fangshi
        /// </summary>
        public string request_method { get; set; }
        /// <summary>
        /// 最后一次执行时间
        /// </summary>
        public DateTime? last_time { get; set; }
        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }


    }
}
