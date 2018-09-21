using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon.TaskScheduler
{
    public enum PlanType
    {
        Once = 1,
        Repeat = 2
    }
    /// <summary>
    /// 计划任务
    /// </summary>
    public class Plan
    {
        /// <summary>
        /// 计划类型
        /// </summary>
        public PlanType Type { get; set; }
        /// <summary>
        /// 计划的开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 计划的结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 单位每1秒
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 关联的方法
        /// </summary>
        public Action<object> ExecuteRelate { get; set; }

        /// <summary>
        /// 已经执行的次数
        /// </summary>
        private int excuteTimes;

        public bool CanExcute()
        {
            DateTime nowtime = DateTime.Now;
            if (nowtime < StartTime || nowtime > EndTime)
                return false;
            if (Type == PlanType.Once)
            {
                if (excuteTimes == 0)
                    return true;
            }
            else
            {
                DateTime preTime = StartTime.AddSeconds(excuteTimes * Interval);
                int totalhours = (int)Math.Floor((nowtime - preTime).TotalSeconds);
                if (totalhours >= 1)
                    return true;
            }
            return false;
        }

        public void Excute(object obj)
        {
            if (!CanExcute())
                return;
            ExecuteRelate(obj);
            excuteTimes++;
        }
    }
}
