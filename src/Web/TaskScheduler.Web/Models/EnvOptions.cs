using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskScheduler.Web.Models
{
    public class EnvOptions
    {
        public string id { get; set; }
        public string key { get; set; }
        public string val { get; set; }
        public bool isSet { get; set; } 
        public bool _temporary { get; set; } 
        public DateTime create_time { get; set; }
    }
}
