using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCEEC.Numerics;
using SCEEC.MI.High_Precision;

namespace HV9003TE4.Models
{
    public class MeasureResult
    {
        public TaskPanelResult PanelResultOne { get; set; } = new TaskPanelResult();
        public TaskPanelResult PanelResultTwo { get; set; } = new TaskPanelResult();
        public TaskPanelResult PanelResultThree { get; set; } = new TaskPanelResult();
        public TaskPanelResult PanelResultFour { get; set; } = new TaskPanelResult();
        public byte Fre { get; set; }
        public PhysicalVariable TestSpeed { get; set; }
    }
    public class TaskPanelResult
    {
        public List<VolateDataKind> Volatepointresult { get; set; } = new List<VolateDataKind>();
        public PhysicalVariable DYVolate { get; set; }
        public bool DyQuatity { get; set; }
        public int ImagDyId { get; set; }
        public PhysicalVariable KeepVolated { get; set; }
        public int KeepTimed { get; set; }
        public int ImagKeepVolateIdd { get; set; }
     
    }


    public class VolateDataKind
    {
        public PhysicalVariable Volate { get; set; }
        public PhysicalVariable Cn { get; set; }
        public PhysicalVariable CnTan { get; set; }
    }


}
