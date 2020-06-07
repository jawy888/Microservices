using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using TaskScheduler.Web.Configs;
using TaskScheduler.Web.Extensions;
using TaskScheduler.Web.Infrastructure.Attr;
using TaskScheduler.Web.Infrastructure.Repository;
using TaskScheduler.Web.Models;

namespace TaskScheduler.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public TaskController(ISchedulerFactory schedulerFactory)
        {
            this._schedulerFactory = schedulerFactory;
        }

        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        [Route("all")]
        [HttpGet]
        public IActionResult All(string name, string groupName)
        {
            return Ok(TaskRepository.GetTaskList(name, groupName));
        }

        /// <summary>
        /// 获取作业运行日志
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        [Route("run-log")]
        [HttpGet]
        public IActionResult RunLog(string id)
        {
            return Ok(TaskRepository.GetTaskLog(id));
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        [TaskAuthor]
        public async Task<IActionResult> Add([FromQuery]TaskOptions taskOptions, string id, string name)
        {
            return Ok(await taskOptions.AddJob(_schedulerFactory));
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Route("del")]
        [HttpPost]
        [TaskAuthor]
        public async Task<IActionResult> Remove([FromQuery]TaskOptions taskOptions)
        {
            return Ok(await _schedulerFactory.Remove(taskOptions));
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPost]
        [TaskAuthor]
        public async Task<IActionResult> Update([FromQuery]TaskOptions taskOptions)
        {
            return Ok(await _schedulerFactory.Update(taskOptions));
        }

        /// <summary>
        /// 暂停作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Route("pause")]
        [HttpPost]
        [TaskAuthor]
        public async Task<IActionResult> Pause([FromQuery]TaskOptions taskOptions)
        {
            return Ok(await _schedulerFactory.Pause(taskOptions));
        }

        /// <summary>
        /// 开启作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Route("start")]
        [HttpPost]
        [TaskAuthor]
        public async Task<IActionResult> Start([FromQuery]TaskOptions taskOptions)
        {
            return Ok(await _schedulerFactory.Start(taskOptions));
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [Route("run")]
        [HttpPost]
        [TaskAuthor]
        public async Task<IActionResult> Run([FromQuery]TaskOptions taskOptions)
        {
            return Ok(await _schedulerFactory.Run(taskOptions));
        }

        [Route("env-list")]
        [HttpGet]
        public IActionResult GetEnv()
        {
            var list = TaskRepository.GetEnv();
            list.ForEach(x =>
            {
                x.isSet = true;
                x._temporary = false;
            });
            return Ok(list);
        }

        [Route("del-env")]
        [HttpPost]
        [TaskAuthor]
        public IActionResult DelEnv([FromQuery]string id)
        {
            var result = TaskRepository.DeleteEnv(id);
            return Ok(new { status = true, msg = "操作成功" });
        }

        [Route("opera-env")]
        [HttpPost]
        [TaskAuthor]
        public IActionResult OperaEnv([FromQuery]EnvOptions env, [FromQuery] bool isSet)
        {
            if (isSet)
                TaskRepository.AddEnv(env);
            else
                TaskRepository.UpdateEnv(env);
            return Ok(new { status = true, msg = "操作成功" });
        }

        [Route("groupname-list")]
        [HttpGet]
        public IActionResult GetGroupName()
        {
            var list = TaskRepository.GetTaskGroupName();
            var handList = new List<dynamic>();
            foreach (var item in list)
            {
                handList.Add(new
                {
                    group_name = item
                });
            }

            return Ok(handList);
        }
    }
}