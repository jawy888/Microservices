using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Consul.Customization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BaskectSettings _baskectSettings;
        public BasketController(IHttpClientFactory httpClientFactory,
            IOptionsSnapshot<BaskectSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _baskectSettings = options.Value;
        }

        [Route("catalog-list")]
        [HttpGet]
        public async Task<IActionResult> GetProductListAsync()
        {
            var service = await ConsulHelper.GetServerAsync(_baskectSettings.CatalogServerName);
            var client = _httpClientFactory.CreateClient();
            var url = $"http://{service.ServiceAddress}:{service.ServicePort}/api/catalog/list";
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }

        [Route("config")]
        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            return Ok(_baskectSettings);
        }

        [Route("hw")]
        [HttpGet]
        public async Task<IActionResult> Huawei()
        {
            var client = _httpClientFactory.CreateClient();
            var image1 = await client.GetAsync("https://i.ibb.co/jT6FQmQ/1591000523-1.png");
            var b1 = await image1.Content.ReadAsByteArrayAsync();
            var base1 = Convert.ToBase64String(b1);

            var image2 = await client.GetAsync("https://i.ibb.co/Yf5gp5n/1591000543-1.png");
            var b2 = await image2.Content.ReadAsByteArrayAsync();
            var base2 = Convert.ToBase64String(b2);

            var requestMessage = new HttpRequestMessage();
            //requestMessage.Headers.Add("Content-Type","application/json");
            requestMessage.RequestUri = new Uri("https://iam.ap-southeast-1.myhuaweicloud.com/v2/08e2b3ec178010672f48c00dd565c3d5/face-compare");
            requestMessage.Method = HttpMethod.Post;
            var body = new
            {
                image1_base64 = base1,
                image2_base64 = base2
            };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            requestMessage.Content = content;

            var response = await client.SendAsync(requestMessage);

            var reuslt = await response.Content.ReadAsStringAsync();
            return Ok(reuslt);
        }
    }
}