using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TaskScheduler.Web.Extensions;
using TaskScheduler.Web.Infrastructure.Repository;
using TaskScheduler.Web.Infrastructure.Utility;
using TaskScheduler.Web.Models;

namespace TaskScheduler.Web.Infrastructure.Jobs
{
    public class HttpResultful : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string httpMessage = string.Empty, requestUri = string.Empty;
            TaskOptions taskOptions = context.GetTaskOptions();
            if (taskOptions == null)
            {
                httpMessage = "No task found";
                //没有找到任务
                await Task.CompletedTask;
            }
            if (string.IsNullOrEmpty(taskOptions.api_url))
            {
                httpMessage = "uri is empty";
                //远程地址为空
                await Task.CompletedTask;
            }
            else
            {
                //环境变量处理
                if (taskOptions.api_url.Contains("{"))
                {
                    var startIndex = taskOptions.api_url.IndexOf("{") + 1;
                    var endIndex = taskOptions.api_url.IndexOf("}");
                    var envName = taskOptions.api_url.Substring(startIndex, endIndex - startIndex);
                    var envInfo = TaskRepository.GetEnvByKey(envName);
                    requestUri = taskOptions.api_url.Replace("{" + envName + "}", envInfo.val);
                }
                else
                {
                    requestUri = taskOptions.api_url;
                }
            }
            var execute_time = DateTime.Now;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.RequestUri = new Uri(requestUri);
                switch (taskOptions.request_method?.ToLower())
                {
                    case "get":
                        httpRequestMessage.Method = HttpMethod.Get;
                        break;
                    case "post":
                        httpRequestMessage.Method = HttpMethod.Post;
                        break;
                }
                if (!string.IsNullOrEmpty(taskOptions.auth_key)
                    && !string.IsNullOrEmpty(taskOptions.auth_value))
                {
                    httpRequestMessage.Headers.Add(taskOptions.auth_key.Trim(), taskOptions.auth_value.Trim());
                }
                var response = await HttpManager.httpClient.SendAsync(httpRequestMessage);
                httpMessage = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpMessage = $"error >>>{ex.Message}";
            }
            if (httpMessage.Length > 500)
                httpMessage = httpMessage.Substring(0, 498);
            stopWatch.Stop();
            taskOptions.last_time = execute_time;
            TaskRepository.UpdateTask(taskOptions);
            TaskRepository.AddTaskLog(new TaskLog
            {
                id = Guid.NewGuid().ToString("N"),
                task_id = taskOptions.id,
                execute_time = execute_time,
                apm = stopWatch.ElapsedMilliseconds,
                response = httpMessage
            });
            await Task.CompletedTask;
        }
    }
}
