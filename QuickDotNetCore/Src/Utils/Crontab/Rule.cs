using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TimerTasker.Crontab
{
    /*
     * 实现类Demo
     * 
      public class TimerTask : BaseCrontab
    {
        public AcceptFriendTimerTask(string id, Rule rule)
        {
            this.id = id;
            this.rule = rule;
        }
        private string id;
        public override object Id => id;
        private Rule rule = null;
        public override Rule Rule => rule;

        public override void Task()
        {
            using var scope = ServiceLocator.Services.CreateScope();
            IWsService wsService = scope.ServiceProvider.GetService(typeof(IWsService)) as IWsService;  //获取这个service
        }
    }

     调用Demo
     Rule rule1 = Rule.BuildLoopRule(RuleAccuracy.Minute, (d) => d.Hour == 1));
     ChatTimerTask timerTask = new(TaskId, rule1);
     CrontabManager.RegisterTask(timerTask);
     */



    /// <summary>
    /// 规则模式
    /// </summary>
    public enum RuleModel { 
        /// <summary>
        /// 只执行一次  然后自动卸载
        /// </summary>
        Once,
        /// <summary>
        /// 无限循环执行
        /// </summary>
        Loop,
        /// <summary>
        /// 执行指定次数
        /// </summary>
        ManyTimes
    }

    /// <summary>
    /// 规则精度
    /// </summary>
    public enum RuleAccuracy {
        Day,
        Hour,
        Minute,
        Second
    }
    public class Rule
    {
        /// <summary>
        /// 运行模式
        /// </summary>
        public RuleModel Model { get; private set; }

        /// <summary>
        /// 精度
        /// </summary>
        public RuleAccuracy RuleAccuracy { get; private set; }

        /// <summary>
        /// 时间规则表达式
        /// </summary>
        public Func<DateTime, bool> TimeExpression { get; private set; }

        /// <summary>
        /// 指定运行时间点 (只适用于单次运行模式(once))
        /// </summary>
        public DateTime DateTime { get; private set; }


        /// <summary>
        /// 运行次数
        /// </summary>
        public int RunTimes { get; private set; } = -1;


        private Rule(RuleAccuracy accuracy) {
            this.RuleAccuracy = accuracy;
        }

        private Rule()
        {
          
        }

        /// <summary>
        /// 构建一个只执行一次的定时任务 
        /// </summary>
        /// <param name="dateTime">指定时间</param>
        /// <returns></returns>
        public static Rule BuildOnceRule(DateTime dateTime) {
            Rule rule = new();
            rule.DateTime = dateTime;
            rule.Model = RuleModel.Once;
            rule.RunTimes = 1;
            return rule;
        }


        /// <summary>
        /// 构建一个循环执行的定时任务
        /// </summary>
        /// <param name="timeExpression">时间条件表达式</param>
        /// <param name="accuracy">执行精度</param>
        /// <returns></returns>
        public static Rule BuildLoopRule(RuleAccuracy accuracy, Func<DateTime, bool> timeExpression)
        {
            Rule rule = new(accuracy);
            rule.TimeExpression = timeExpression;
            rule.Model = RuleModel.Loop;
            return rule;
        }


        /// <summary>
        /// 构建一个执行指定次数的定时任务
        /// </summary>
        /// <param name="runTimes">执行次数</param>
        /// <param name="accuracy">执行精度</param>
        /// <param name="timeExpression">时间条件表达式</param>
        /// <returns></returns>
        public static Rule BuildManyTimesRule(int runTimes, RuleAccuracy accuracy, Func<DateTime, bool> timeExpression)
        {
            Rule rule = new(accuracy);
            rule.TimeExpression = timeExpression;
            rule.RunTimes = runTimes;
            rule.Model = RuleModel.ManyTimes;
            return rule;
        }
    }
}
