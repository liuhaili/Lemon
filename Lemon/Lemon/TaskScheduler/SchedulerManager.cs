using System;
using System.Collections.Generic;
using System.Threading;

namespace Lemon.TaskScheduler
{
    /// <summary>
    /// 调度管理
    /// </summary>
    public class SchedulerManager : SingletonBase<SchedulerManager>
    {
        List<Plan> planList = new List<Plan>();
        readonly object planListLock = new object();
        Action<Exception> errorEvent = null;
        Timer timer = null;

        public void Start()
        {
            //启动计划
            timer = new Timer(TimerTick, null, 1, 1000);  //1秒钟执行一次
            Console.WriteLine("计划任务已开启");
        }

        public void Stop()
        {
            if (timer != null)
                timer.Change(int.MaxValue, int.MaxValue);
        }

        public void Regist(Plan plan)
        {
            lock (planListLock)
            {
                planList.Add(plan);
            }
        }

        public void SetErrorEvent(Action<Exception> errorevent)
        {
            errorEvent = errorevent;
        }

        public void TimerTick(object state)
        {
            lock (planListLock)
            {
                foreach (Plan p in planList)
                {
                    try
                    {
                        p.Excute(null);
                    }
                    catch (Exception ex)
                    {
                        if (errorEvent != null)
                            errorEvent(ex);
                    }
                }
            }
        }
    }
}
