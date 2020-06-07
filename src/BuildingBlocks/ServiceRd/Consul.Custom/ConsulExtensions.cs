using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Winton.Extensions.Configuration.Consul;

namespace Consul.Customization
{
    public static class ConsulExtensions
    {

        #region 基础属性
        //Consul服务器IP
        internal static string consulIP;
        //Consul服务器端口
        internal static string consulPort;
        //Consul HTTP地址
        internal static string consulServerUrl;
        //API接口服务器IP
        internal static string serverIP;
        //API接口服务器端口
        internal static string serverPort;
        //服务名称
        internal static string serverName;
        //注册Consul服务名称
        internal static string consulServiceName;
        //注册Consuld配置中心配置名称
        internal static string consulServiceConfigName;
        //系统前缀
        internal static string systemPrefix;
        //是否开启健康检查
        internal static bool openHealth;
        internal static string consulScheme;
        //ACL Token
        internal static string aclToken;
        //是否使用Consul配置中心
        internal static bool isUseConsulConfigCenter = false;
        #endregion

        /// <summary>
        /// 使用Consul并且注册当前API服务
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            #region check consul
            if (!isUseConsulConfigCenter)
            {

            }
            if (string.IsNullOrEmpty(consulIP))
                throw new Exception("配置项Consul IP为空--->Consul:IP");

            if (string.IsNullOrEmpty(consulPort))
                throw new Exception("配置项Consul 端口为空--->Consul:Port");

            if (string.IsNullOrEmpty(serverIP))
                throw new Exception("配置项服务IP为空--->Server:IP");

            if (string.IsNullOrEmpty(serverPort))
                throw new Exception("配置项服务端口为空--->Server:Port");
            #endregion

            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            using (var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(consulServerUrl);
                if (!string.IsNullOrEmpty(aclToken))
                    x.Token = aclToken;
            }))
            {

                var config = consulClient.KV.Get(consulServiceConfigName).Result;


                var ocelotConfig = consulClient.KV.Get("InternalConfiguration").Result;
                var ocelotConfigVal = Encoding.UTF8.GetString(ocelotConfig.Response.Value, 0, ocelotConfig.Response.Value.Length);


                if (config.StatusCode == HttpStatusCode.NotFound)
                {
                    var putPair = new KVPair(consulServiceConfigName);
                    var putAttempt = consulClient.KV.Put(putPair).Result;
                }
                //服务名称
                var registration = new AgentServiceRegistration()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    Name = consulServiceName,
                    Address = serverIP,
                    Port = Convert.ToInt32(serverPort)
                };

                if (openHealth)
                {
                    registration.Check = new AgentServiceCheck()
                    {
                        //服务启动多久后注册
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                        //健康检查时间间隔
                        Interval = TimeSpan.FromSeconds(10),
                        //健康检查地址
                        HTTP = $"http://{serverIP}:{serverPort}/api/health",
                        Timeout = TimeSpan.FromSeconds(5)
                    };
                }
                // 注册服务
                consulClient.Agent.ServiceRegister(registration).Wait();
                // 应用程序终止时，注销服务
                lifetime.ApplicationStopping.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
                return app;
            }
        }

        /// <summary>
        /// Consul装载配置
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder ConsulConfigureCenter(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                isUseConsulConfigCenter = true;
                var env = hostingContext.HostingEnvironment;
                hostingContext.Configuration = config.Build();
                systemPrefix = hostingContext.Configuration["Server:SystemPrefix"];
                if (string.IsNullOrEmpty(systemPrefix))
                    throw new Exception("系统前缀为空");
                aclToken = hostingContext.Configuration["Consul:ACLToken"];
                if (string.IsNullOrEmpty(aclToken))
                    throw new Exception("ACL令牌为空");
                consulIP = hostingContext.Configuration["Consul:IP"];
                consulPort = hostingContext.Configuration["Consul:Port"];
                consulScheme = Convert.ToBoolean(hostingContext.Configuration["Consul:IsHttps"]) ? "https" : "http";
                consulServerUrl = $"{consulScheme}://{consulIP}:{consulPort}";
                serverIP = hostingContext.Configuration["Server:IP"];
                serverPort = hostingContext.Configuration["Server:Port"];
                openHealth = Convert.ToBoolean(hostingContext.Configuration["Server:OpenHealth"]);
                serverName = env.ApplicationName;
                consulServiceName = $"{systemPrefix}.{serverName}.service".ToLower();
                consulServiceConfigName = $"{systemPrefix}.{serverName}.config".ToLower();
                config.AddConsul(
                            consulServiceConfigName,
                            options =>
                            {
                                // 获取或设置一个值，该值指示配置是否可选。
                                options.Optional = true;
                                //获取或设置一个值，该值指示如果领事中的数据发生更改，是否将重新加载源。
                                options.ReloadOnChange = true;
                                //获取或设置在配置加载期间引发异常时调用的System.Action 客户端使用它来处理异常（如果可能）并防止引发异常。
                                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                                //获取或设置在Consul.IConsulClient构建期间要应用于Consul.ConsulClientConfiguration的System.Action。 允许覆盖Consul的默认配置选项。
                                options.ConsulConfigurationOptions = cco =>
                                {
                                    cco.Address = new Uri(consulServerUrl);
                                    if (!string.IsNullOrEmpty(aclToken))
                                        cco.Token = aclToken;

                                };
                            });
                hostingContext.Configuration = config.Build();
            });
            return hostBuilder;
        }

    }
}
