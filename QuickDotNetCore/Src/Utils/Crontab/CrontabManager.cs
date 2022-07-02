using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimerTasker.Crontab
{
    /// <summary>
    /// 定时任务管理器  精度应该不是很高
    /// </summary>
    public class CrontabManager
    {
        private readonly static List<ICrontab> crontabs = new();

        private readonly static object lockObj = new();

        /// <summary>
        /// 扫描间隔
        /// </summary>
        private const int scanInterval = 50;//ms

        /// <summary>
        /// 退出标记
        /// </summary>
        private readonly static bool exitFlag = false;


        static CrontabManager()
        {
            Run();
        }

        /// <summary>
        /// 注册任务
        /// </summary>
        /// <param name="crontab"></param>
        public static void RegisterTask(ICrontab crontab)
        {
            lock (lockObj)
            {
                ICrontab? crontab1 = crontabs.FirstOrDefault((d) => d.Id.Equals(crontab.Id));
                if (crontab1 != null)
                {
                    crontabs.Remove(crontab1);
                }
                crontabs.Add(crontab);
            }
        }
        /// <summary>
        /// 修改任务时间(不可用)
        /// </summary>
        public static void UpdateTaskRule(string Id,Rule rule)
        {
            lock (lockObj)
            { 
                ICrontab? crontab1 = crontabs.FirstOrDefault((d) => d.Id.Equals(Id));
                if (crontab1 != null)
                {
                    crontabs.Add(crontab1);
                }
            }
        }
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveTask(object id)
        {
            lock (lockObj)
            {
                ICrontab crontab = crontabs.FirstOrDefault((d) => d.Id .Equals( id));
                if (crontab != null)
                {
                    crontabs.Remove(crontab);
                    crontab.Dump();
                }
            }
        }


        private static void Run()
        {
            Thread thread = new(() =>
            {
                while (!exitFlag)
                {
                    Thread.Sleep(scanInterval);
                    crontabs.ToList().ForEach((c) =>
                    {
                        if (c.State == CrontabState.Idle && CheckRule(c))
                        {
                            RunTask(c);
                            Thread.Sleep(scanInterval*2);
                        }
                    });
                    lock (lockObj)
                    {
                        //移除所有已完成并丢弃的任务
                        crontabs.RemoveAll((d) => d.State == CrontabState.Dump);
                    }
                }
            });
            thread.Start();
        }


        private static void RunTask(ICrontab crontab)
        {
            if (crontab.Rule.Model == RuleModel.Once)
            {
                crontab.Dump();
            }
            ThreadPool.QueueUserWorkItem((o) => crontab.RunTask());
        }


        private static bool CheckRule(ICrontab crontab)
        {
            //这里面可能有问题
            Rule rule = crontab.Rule;
            DateTime dateTime = DateTime.Now;
            if (!crontab.IsFirstTime && (dateTime - crontab.LastRunDateTime).TotalMilliseconds <= CaleInterval(crontab.Rule.RuleAccuracy))
            {
                return false;
            }
            if (rule.Model != RuleModel.Once)
            {
                return rule.TimeExpression(dateTime);
            }
            else
            {
                TimeSpan timeSpan = rule.DateTime - dateTime;
                return timeSpan.TotalMilliseconds < scanInterval * 5;
            }
        }

        private static double CaleInterval(RuleAccuracy accuracy)
        {
            double r = 0;
            switch (accuracy)
            {
                case RuleAccuracy.Day:
                    r = TimeSpan.FromDays(1).TotalMilliseconds + scanInterval;
                    break;
                case RuleAccuracy.Hour:
                    r = TimeSpan.FromHours(1).TotalMilliseconds + scanInterval;
                    break;
                case RuleAccuracy.Minute:
                    r = TimeSpan.FromMinutes(1).TotalMilliseconds + scanInterval;
                    break;
                case RuleAccuracy.Second:
                    r = TimeSpan.FromSeconds(1).TotalMilliseconds;
                    break;
                default:
                    break;
            }
            return r;
        }
    }
}
