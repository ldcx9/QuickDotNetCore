using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimerTasker.Crontab
{
    public interface ICrontab
    {
        /// <summary>
        /// 是否首次运行
        /// </summary>
        bool IsFirstTime { get; }

        /// <summary>
        /// 最后运行时间
        /// </summary>
        DateTime LastRunDateTime { get; }

        /// <summary>
        /// 任务状态
        /// </summary>
        CrontabState State { get; }

        /// <summary>
        /// 任务ID
        /// </summary>
        object Id { get; }

        /// <summary>
        /// 任务规则
        /// </summary>
        Rule Rule { get; }


        /// <summary>
        /// 抛弃任务
        /// </summary>
        void Dump();

        /// <summary>
        /// 暂停任务
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复任务
        /// </summary>
        void Resume();

        /// <summary>
        /// 任务  会在线程池内运行  要自行处理线程安全问题
        /// </summary>
        void Task();

        /// <summary>
        /// 执行任务
        /// </summary>
        void RunTask();
    }
}
