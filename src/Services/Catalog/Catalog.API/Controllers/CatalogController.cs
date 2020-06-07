using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        [Route("list")]
        [HttpGet]
        public IActionResult List()
        {
            var list = CatalogItem.GetList();
            return Ok(list);
        }

        public IActionResult Get() => Ok("I'm service1");
    }
}