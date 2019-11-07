using System.Collections.Generic;
using SCEEC.Numerics;

namespace HV9003TE4.Models
{
    public class AllPanelResult
    {
        List<VolateDataKind> Volatepointresult { get; set; } = new List<VolateDataKind>();
        public PhysicalVariable EleyVolate { get; set; }
        public bool EleyQuatity { get; set; }
        public int ImagEleyId { get; set; }
        public PhysicalVariable KeepVolate { get; set; }
        public int KeepTime { get; set; }
        public int ImagKeepVolateId { get; set; }
        public byte Fre { get; set; }
        public PhysicalVariable TestSpeed { get; set; }
    }

  
}
