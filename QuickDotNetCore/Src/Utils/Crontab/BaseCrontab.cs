using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimerTasker.Crontab
{
    public abstract class BaseCrontab : ICrontab
    {
        public bool IsFirstTime { get; private set; } = true;

        public DateTime LastRunDateTime { get; private set; } = DateTime.Now;

        public CrontabState State { get; private set; } = CrontabState.Idle;

        public abstract object Id { get; }

        public abstract Rule Rule { get; }

        public abstract void Task();

        public void RunTask() {
            IsFirstTime = false;
            LastRunDateTime = DateTime.Now;

            Task();
        }

        /// <summary>
        /// 丢弃该掉任务(正在执行的不可打断)  不可恢复
        /// </summary>
        public void Dump()
        {
            this.State = CrontabState.Dump;
        }

        public void Pause()
        {
            this.State = CrontabState.Pause;
        }

        public void Resume()
        {
            this.State = CrontabState.Idle;
        }
    }
}
