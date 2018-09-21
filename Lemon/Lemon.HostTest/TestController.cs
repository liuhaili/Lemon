using Lemon.InvokeRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.Test
{
    public class TestController : IActionController
    {
        [Action]
        public string GetAge(string name, int age)
        {
            return name+" is Back";
        }
    }
}
