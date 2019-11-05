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

        public bool WaveImageState { get; set; } = false;
        public Int16 WaveImageId { get; set; }

        public int VolateNUm { get; set; } = 1;//耐压之前有多少点

        public double VolateSpeed { get; set; } = 2;
    }

    public static class AutoStateStatic
    {
        public static AutoState SState = new AutoState();
    }
}
