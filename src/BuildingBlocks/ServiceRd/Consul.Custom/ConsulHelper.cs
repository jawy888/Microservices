using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consul.Customization
{
    public static class ConsulHelper
    {
        #region 根据服务名获取注册中心已注册的服务
        /// <summary>
        /// 根据服务名获取Consul可用的服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static async Task<CatalogService> GetServerAsync(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new Exception("服务名为空");
            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(ConsulExtensions.consulServerUrl);
                if (!string.IsNullOrEmpty(ConsulExtensions.aclToken))
                    x.Token = ConsulExtensions.aclToken;
            }))
            {
                var servicesRequest = await consulClient.Catalog.Service(serviceName);
                var services = servicesRequest.Response;
                if (services != null && services.Any())
                {
                    var serviceCount = services.Count();
                    var index = 0;
                    if (serviceCount > 1)
                    {
                        var radom = new Random();
                        index = radom.Next(serviceCount);
                    }

                    var service = services.ElementAt(index);
                    return service;
                }
                throw new Exception("该服务名下没有任何可用的服务");
            }
        }

        /// <summary>
        /// 根据服务名获取Consul可用的服务
        /// </summary>
        /// <param name="serverName">注册的服务名</param>
        /// <returns></returns>
        public static CatalogService GetServer(string serverName)
        {
            if (string.IsNullOrEmpty(serverName))
                throw new Exception("服务名为空");
            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(ConsulExtensions.consulServerUrl);
                if (!string.IsNullOrEmpty(ConsulExtensions.aclToken))
                    x.Token = ConsulExtensions.aclToken;
            }))
            {
                var servicesRequest = consulClient.Catalog.Service(serverName).Result;
                var services = servicesRequest.Response;
                if (services != null && services.Any())
                {
                    var serviceCount = services.Count();
                    var index = 0;
                    if (serviceCount > 1)
                    {
                        var radom = new Random();
                        index = radom.Next(serviceCount);
                    }
                    var service = services.ElementAt(index);
                    return service;
                }
                throw new Exception("该服务名下没有任何可用的服务");
            }
        }
        #endregion

        #region 注销服务
        /// <summary>
        /// 注销服务(注:仅限当前应用)
        /// </summary>
        /// <param name="serverId">注册的服务ID</param>
        /// <returns></returns>
        public static async Task ServiceDeregisterAsync(string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new Exception("服务Id为空");

            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(ConsulExtensions.consulServerUrl);
                if (!string.IsNullOrEmpty(ConsulExtensions.aclToken))
                    x.Token = ConsulExtensions.aclToken;
            }))
            {

                var servicesRequest = await consulClient.Catalog.Service(ConsulExtensions.consulServiceName);
                var services = servicesRequest.Response;
                var service = services.Where(x => x.ServiceID == serverId)
                              .SingleOrDefault();
                if (service != null)
                {
                    consulClient.Agent.ServiceDeregister(service.ServiceID).Wait();
                }
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// 注销服务(注:仅限当前应用)
        /// </summary>
        /// <param name="serverId">注册的服务ID</param>
        /// <returns></returns>
        public static void ServiceDeregister(string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new Exception("服务Id为空");

            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(ConsulExtensions.consulServerUrl);
                if (!string.IsNullOrEmpty(ConsulExtensions.aclToken))
                    x.Token = ConsulExtensions.aclToken;
            }))
            {

                var servicesRequest = consulClient.Catalog.Service(ConsulExtensions.consulServiceName).Result;
                var services = servicesRequest.Response;
                var service = services.Where(x => x.ServiceID == serverId)
                              .SingleOrDefault();
                if (service != null)
                {
                    consulClient.Agent.ServiceDeregister(service.ServiceID).Wait();
                }
            }

        }

        /// <summary>
        /// 注销全部服务(注:仅限当前应用)
        /// </summary>
        /// <returns></returns>
        public static async Task AllServiceDeregisterAsync()
        {
            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(ConsulExtensions.consulServerUrl);
                if (!string.IsNullOrEmpty(ConsulExtensions.aclToken))
                    x.Token = ConsulExtensions.aclToken;
            }))
            {
                var servicesRequest = await consulClient.Catalog.Service(ConsulExtensions.consulServiceName);
                var services = servicesRequest.Response;
                foreach (var item in services)
                {
                    consulClient.Agent.ServiceDeregister(item.ServiceID).Wait();
                }
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// 注销全部服务(注:仅限当前应用)
        /// </summary>
        /// <returns></returns>
        public static void AllServiceDeregister()
        {
            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(ConsulExtensions.consulServerUrl);
                if (!string.IsNullOrEmpty(ConsulExtensions.aclToken))
                    x.Token = ConsulExtensions.aclToken;
            }))
            {
                var servicesRequest = consulClient.Catalog.Service(ConsulExtensions.consulServiceName).Result;
                var services = servicesRequest.Response;
                foreach (var item in services)
                {
                    consulClient.Agent.ServiceDeregister(item.ServiceID).Wait();
                }
            }
        }

        /// <summary>
        /// 注销全部服务(注:仅限当前应用)
        /// </summary>
        /// <returns></returns>
        public static void AllServiceDeregister(this IApplicationBuilder app)
        {
            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(ConsulExtensions.consulServerUrl);
                if (!string.IsNullOrEmpty(ConsulExtensions.aclToken))
                    x.Token = ConsulExtensions.aclToken;
            }))
            {
                var servicesRequest = consulClient.Catalog.Service(ConsulExtensions.consulServiceName).Result;
                var services = servicesRequest.Response;
                foreach (var item in services)
                {
                    consulClient.Agent.ServiceDeregister(item.ServiceID).Wait();
                }
            }
        }
        #endregion
    }
}
