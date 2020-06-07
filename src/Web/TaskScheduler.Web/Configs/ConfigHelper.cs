using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Web.Extensions;

namespace TaskScheduler.Web.Configs
{
    public class ConfigHelper
    {
        public static IConfiguration _configuration;
        public static void SetInstance(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static IConfiguration GetInstance()
        {
            return _configuration;
        }
    }
}
