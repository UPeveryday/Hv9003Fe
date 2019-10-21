using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCEEC.Numerics;

namespace HV9003TE4.Models
{
    public class FourTestResult
    {
        public int NeedTestNum { get; set; }
        public List<PanelResult> Panel1Result { get; set; } = new List<PanelResult>();
        public List<PanelResult> Panel2Result { get; set; } = new List<PanelResult>();
        public List<PanelResult> Panel3Result { get; set; } = new List<PanelResult>();
        public List<PanelResult> Panel4Result { get; set; } = new List<PanelResult>();
        public PanelEleYAndEleVolate Panel1EleYAndVolate { get; set; } = new PanelEleYAndEleVolate();
        public PanelEleYAndEleVolate Panel2EleYAndVolate { get; set; } = new PanelEleYAndEleVolate();
        public PanelEleYAndEleVolate Panel3EleYAndVolate { get; set; } = new PanelEleYAndEleVolate();
        public PanelEleYAndEleVolate Panel4EleYAndVolate { get; set; } = new PanelEleYAndEleVolate();
        public bool PanelEnable { get; set; } = false;
        public bool Pane2Enable { get; set; } = false;
        public bool Pane3Enable { get; set; } = false;
        public bool Pane4Enable { get; set; } = false;
        public void Clear()
        {
            Panel1Result.Clear();
            Panel2Result.Clear();
            Panel3Result.Clear();
            Panel4Result.Clear();
            Panel1EleYAndVolate.Clear();
            Panel2EleYAndVolate.Clear();
            Panel3EleYAndVolate.Clear();
            Panel4EleYAndVolate.Clear();

        }
    }

    public class PanelResult
    {
        public PhysicalVariable Cn { get; set; }
        public PhysicalVariable CnTan { get; set; }
    }
    public class PanelEleYAndEleVolate
    {
        public PhysicalVariable EleY { get; set; }
        public PhysicalVariable EleVolate { get; set; }
        public int HodeTime { get; set; }

        public void Clear()
        {
            EleY = "0V";
            EleVolate = "0V";
            HodeTime = 0;
        }
    }
}
