using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class SharedClass
    {
        public static double[] dopplerShifts; // 设为 public 和 static 以便跨项目访问
        public static double[] gateNumbers;
    }
}
