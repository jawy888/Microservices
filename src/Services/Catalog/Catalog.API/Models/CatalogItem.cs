using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class CatalogItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public static List<CatalogItem> GetList()
        {
            var list = new List<CatalogItem>
            {
                new CatalogItem
                {
                    Id="cf06d0fc-8d88-44ec-a363-8742f7f0c8aa",
                    Name="阿迪达斯官网 三叶草 SUPERSTAR 男女 贝壳头 经典运动鞋 EG4958",
                    Stock=100
                },
                new CatalogItem
                {
                    Id="cf06d0fc-8d88-44ec-a363-8742f7f0c8ab",
                    Name="阿迪达斯官网COSMIC 2男子跑步运动鞋B44882 B44880 EE8180F34876",
                    Stock=200
                },
                new CatalogItem
                {
                    Id="cf06d0fc-8d88-44ec-a363-8742f7f0c8ac",
                    Name="阿迪达斯官网adidas三叶草男装运动短袖T恤FK1349 DV1603 EJ9677",
                    Stock=300
                }
            };
            return list;
        }
    }
}
