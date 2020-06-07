using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskScheduler.Web.Infrastructure.Enum
{
    public enum TaskAction
    {
        新增=1,
        删除,
        修改,
        暂停,
        停止,
        开启,
        立即执行
    }
}
