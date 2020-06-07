using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API
{
    public class BaskectSettings
    {
        public string Redis { get; set; }
        public string MySQL { get; set; }
        public string CatalogServerName { get; set; }
        public string Test { get; set; }
    }
}
