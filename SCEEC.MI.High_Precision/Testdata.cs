using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEEC.MI.High_Precision
{
    public interface Testdata
    {
        string TestPanel { get; set; }
        string TestSpeed { get; set; }
        string TestCurrent { get; set; }
        string TestMeasuredCurrent { get; set; }
        string TestAngle { get; set; }
        string TestFre { get; set; }
    }
}
