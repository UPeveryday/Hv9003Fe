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
        public PanelEleYAndEleVolate Panel1EleYAndVolate { get; set; } 
        public PanelEleYAndEleVolate Panel2EleYAndVolate { get; set; } 
        public PanelEleYAndEleVolate Panel3EleYAndVolate { get; set; } 
        public PanelEleYAndEleVolate Panel4EleYAndVolate { get; set; }
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
    }
}
