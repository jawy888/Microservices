using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Web.Infrastructure;
using TaskScheduler.Web.Infrastructure.Repository;
using TaskScheduler.Web.Models;

namespace TaskScheduler.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public HealthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [Route("check")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("success");
        }
    }
}