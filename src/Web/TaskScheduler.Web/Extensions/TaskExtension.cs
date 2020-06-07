using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Web.Configs;
using TaskScheduler.Web.Infrastructure;
using TaskScheduler.Web.Infrastructure.Enum;
using TaskScheduler.Web.Infrastructure.Jobs;
using TaskScheduler.Web.Infrastructure.Repository;
using TaskScheduler.Web.Models;

namespace TaskScheduler.Web.Extensions
{
    /// <summary>
    /// 任务扩展类
    /// </summary>
    public static class TaskExtension
    {
        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTaskScheduler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            IServiceProvider services = app.ApplicationServices;
            var _schedulerFactory = (ISchedulerFactory)services.GetService(typeof(ISchedulerFactory));
            TaskRepository.GetTaskList().ForEach(x =>
            {
                var result = x.AddJob(_schedulerFactory, true).Result;
            });
            return app;
        }



        /// <summary>
        /// 添加作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <param name="schedulerFactory"></param>
        /// <param name="init">是否初始化，false持久化，true不持久化</param>
        /// <returns></returns>
        public static async Task<object> AddJob(this TaskOptions taskOptions, ISchedulerFactory schedulerFactory, bool init = false)
        {
            try
            {
                (bool, string) validExpression = taskOptions.interval.IsValidExpression();
                if (!validExpression.Item1)
                    return new { status = false, msg = validExpression.Item2 };

                (bool, object) result = taskOptions.Exists(init);
                if (!result.Item1)
                    return result.Item2;
                if (!init)
                {
                    TaskRepository.AddTask(taskOptions);
                }
                IJobDetail job = JobBuilder.Create<HttpResultful>()
                    .WithIdentity(taskOptions.name, taskOptions.group_name)
                    .Build();
                ITrigger trigger = TriggerBuilder.Create()
                   .WithIdentity(taskOptions.name, taskOptions.group_name)
                   .StartNow().WithDescription(taskOptions.describe)
                   .WithCronSchedule(taskOptions.interval)
                   .Build();
                IScheduler scheduler = await schedulerFactory.GetScheduler();
                await scheduler.ScheduleJob(job, trigger);
                //如果任务处于正常状态
                if (taskOptions.status == (int)TriggerState.Normal)
                {
                    await scheduler.Start();
                }
                else
                {
                    //暂定任务
                    await schedulerFactory.Pause(taskOptions);
                }
            }
            catch (Exception ex)
            {
                return new { status = false, msg = ex.Message };
            }
            return new { status = true };
        }

        /// <summary>
        /// 检查Cron表达式是否有效
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public static (bool, string) IsValidExpression(this string cronExpression)
        {
            try
            {
                var trigger = new CronTriggerImpl();
                trigger.CronExpressionString = cronExpression;
                DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
                return (date != null, date == null ? $"请确认表达式{cronExpression}是否正确!" : "");
            }
            catch (Exception e)
            {
                return (false, $"请确认表达式{cronExpression}是否正确!{e.Message}");
            }
        }

        /// <summary>
        /// 作业是否存在
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <param name="init">初始化的不需要判断</param>
        /// <returns></returns>
        public static (bool, object) Exists(this TaskOptions taskOptions, bool init)
        {
            if (!init && TaskRepository.GetTaskList().Any(x => x.name == taskOptions.name && x.group_name == taskOptions.group_name))
            {
                return (false,
                    new
                    {
                        status = false,
                        msg = $"任务:{taskOptions.name},分组：{taskOptions.group_name}已经存在"
                    });
            }
            return (true, null);
        }

        /// <summary>
        /// 暂停作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task<object> Pause(this ISchedulerFactory schedulerFactory, TaskOptions taskOptions)
        {
            return schedulerFactory.TriggerAction(taskOptions.name, taskOptions.group_name, TaskAction.暂停, taskOptions);
        }

        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static Task<object> Remove(this ISchedulerFactory schedulerFactory, TaskOptions taskOptions)
        {
            return schedulerFactory.TriggerAction(taskOptions.name, taskOptions.group_name, TaskAction.删除, taskOptions);
        }

        /// <summary>
        /// 启动作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task<object> Start(this ISchedulerFactory schedulerFactory, TaskOptions taskOptions)
        {
            return schedulerFactory.TriggerAction(taskOptions.name, taskOptions.group_name, TaskAction.开启, taskOptions);
        }

        /// <summary>
        /// 立即执行一次作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task<object> Run(this ISchedulerFactory schedulerFactory, TaskOptions taskOptions)
        {
            return schedulerFactory.TriggerAction(taskOptions.name, taskOptions.group_name, TaskAction.立即执行, taskOptions);
        }

        /// <summary>
        /// 更新作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task<object> Update(this ISchedulerFactory schedulerFactory, TaskOptions taskOptions)
        {
            return schedulerFactory.TriggerAction(taskOptions.name, taskOptions.group_name, TaskAction.修改, taskOptions);
        }

        /// <summary>
        /// 触发新增、删除、修改、暂停、启用、立即执行事件
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="status"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static async Task<object> TriggerAction(this ISchedulerFactory schedulerFactory, string taskName, string groupName, TaskAction action, TaskOptions taskOptions = null)
        {
            string errorMsg = "";
            try
            {
                IScheduler scheduler = await schedulerFactory.GetScheduler();
                List<JobKey> jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)).Result.ToList();
                if (jobKeys == null || jobKeys.Count() == 0)
                {
                    errorMsg = $"未找到分组[{groupName}]";
                    return new { status = false, msg = errorMsg };
                }
                JobKey jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskName)).FirstOrDefault();
                if (jobKey == null)
                {
                    errorMsg = $"未找到触发器[{taskName}]";
                    return new { status = false, msg = errorMsg };
                }
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskName).FirstOrDefault();

                if (trigger == null)
                {
                    errorMsg = $"未找到触发器[{taskName}]";
                    return new { status = false, msg = errorMsg };
                }
                object result = null;
                switch (action)
                {
                    case TaskAction.删除:
                    case TaskAction.修改:
                        //暂停触发器，移除的触发器,删除触发器相关联的任务
                        await scheduler.PauseTrigger(trigger.Key);
                        await scheduler.UnscheduleJob(trigger.Key);
                        await scheduler.DeleteJob(trigger.JobKey);
                        result = taskOptions.ModifyTaskEntity(schedulerFactory, action);
                        break;
                    case TaskAction.暂停:
                    case TaskAction.停止:
                    case TaskAction.开启:
                        result = taskOptions.ModifyTaskEntity(schedulerFactory, action);
                        if (action == TaskAction.暂停)
                        {
                            await scheduler.PauseTrigger(trigger.Key);
                        }
                        else if (action == TaskAction.开启)
                        {
                            await scheduler.ResumeTrigger(trigger.Key);
                        }
                        else
                        {
                            await scheduler.Shutdown();
                        }
                        break;
                    case TaskAction.立即执行:
                        await scheduler.TriggerJob(jobKey);
                        break;
                }
                return result ?? new { status = true, msg = $"作业{action.ToString()}成功" };
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return new { status = false, msg = ex.Message };
            }
        }

        public static object ModifyTaskEntity(this TaskOptions taskOptions, ISchedulerFactory schedulerFactory, TaskAction action)
        {
            object result = null;
            switch (action)
            {
                case TaskAction.删除:
                    TaskRepository.DeleteTask(new List<string> { taskOptions.id });
                    break;
                case TaskAction.修改:
                    result = taskOptions.AddJob(schedulerFactory, true).GetAwaiter().GetResult();
                    break;
                case TaskAction.暂停:
                case TaskAction.开启:
                case TaskAction.停止:
                case TaskAction.立即执行:
                    if (action == TaskAction.暂停)
                    {
                        taskOptions.status = (int)TriggerState.Paused;
                    }
                    else if (action == TaskAction.停止)
                    {
                        taskOptions.status = (int)action;
                    }
                    else
                    {
                        taskOptions.status = (int)TriggerState.Normal;
                    }
                    break;
            }
            if (action != TaskAction.删除)
            {
                TaskRepository.UpdateTask(taskOptions);
            }

            return result;
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TaskOptions GetTaskOptions(this IJobExecutionContext context)
        {
            var _taskList = TaskRepository.GetTaskList();
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            TaskOptions taskOptions = _taskList.Where(x => x.name == trigger.Name && x.group_name == trigger.Group).FirstOrDefault();
            return taskOptions ?? _taskList.Where(x => x.name == trigger.JobName && x.group_name == trigger.JobGroup).FirstOrDefault();
        }

    }
}
