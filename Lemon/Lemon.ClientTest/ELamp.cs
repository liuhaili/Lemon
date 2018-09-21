using LampMonitor.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LampMonitor.Entity
{
    public class ELamp
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ConcentratorName { get; set; }
        public string Position { get; set; }
        public string Coordinates { get; set; }
        public LampState State { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
