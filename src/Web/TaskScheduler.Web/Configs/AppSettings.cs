using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskScheduler.Web.Configs
{
    public class AppSettings
    {
        /// <summary>
        /// 普通令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 超级令牌
        /// </summary>
        public string SuperToken { get; set; }
    }
}
