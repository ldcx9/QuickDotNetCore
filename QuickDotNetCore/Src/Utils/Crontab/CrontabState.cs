using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimerTasker.Crontab
{
    public enum CrontabState
    {
        /// <summary>
        /// 等待运行
        /// </summary>
        Idle,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 丢弃
        /// </summary>
        Dump
    }
}
