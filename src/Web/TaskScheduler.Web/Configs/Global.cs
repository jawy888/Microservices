using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Web.Extensions;

namespace TaskScheduler.Web.Configs
{
    public class Global
    {
        public static string MySQLConnection;

        static Global()
        {
            var config = ConfigHelper.GetInstance();

            MySQLConnection = config.GetConnectionString("MySQL");
        }
    }
}
