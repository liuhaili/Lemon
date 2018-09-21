using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lemon.InvokeRoute
{
    public class ActionInfo
    {
        public object ControllerObject { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }
}
