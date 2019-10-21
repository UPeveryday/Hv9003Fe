using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HV9003TE4.Models
{
    public class AutoState
    {
        public int Process { get; set; }
        public int TaskCount { get; set; }
        public List<string> TestText { get; set; } = new List<string>();

        public void Clear()
        {
            TestText.Clear();
            Process = 0;
            TaskCount = 0;
            TestText.Clear();
        }
        public bool IsStartEleY { get; set; } = false;
        public bool IsStartVolate { get; set; } = false;

        public int NoSame { get; set; } 
        public int AllNum { get; set; }
        public bool NaiVolate { get; set; } = false;
        public float MaxEqualVolate { get; set; }
        public bool Quality { get; set; }
        public bool CompeleteVolate { get; set; } = false;

        public bool IsPress { get; set; } = false;
    }

    public static class AutoStateStatic
    {
        public static AutoState SState = new AutoState();
    }
}
