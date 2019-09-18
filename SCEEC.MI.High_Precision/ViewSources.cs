using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCEEC.Numerics;

namespace SCEEC.MI.High_Precision
{
    public class ViewSources
    {
        public byte TestSpeed { get; set; }
        public float TestIn { get; set; }
        public float TestIx1 { get; set; }
        public float TestIx2 { get; set; }
        public float TestIx3 { get; set; }
        public float TestIx4 { get; set; }
        public float TestPh1 { get; set; }
        public float TestPh2 { get; set; }
        public float TestPh3 { get; set; }
        public float TestPh4 { get; set; }
        public float TestRn { get; set; }
        public float TestRx1 { get; set; }
        public float TestRx2 { get; set; }
        public float TestRx3 { get; set; }
        public float TestRx4 { get; set; }
        public float TestFre { get; set; } = 50.0f;
        public float TestCurrent { get; set; }
        public float TestPower { get; set; }
        public double TestU0 { get; set; }
        public PhysicalVariable TestCn { get; set; } = "1.002nF";
        public PhysicalVariable TestCnTan { get; set; }
        public double TestCx1 { get; set; }
        public double TestCx2 { get; set; }
        public double TestCx3 { get; set; }
        public double TestCx4 { get; set; }


        public double Testθ0 { get; set; }
        public double TestTan1 { get; set; }
        public double TestTan2 { get; set; }
        public double TestTan3 { get; set; }
        public double TestTan4 { get; set; }
        public double OneVolate { get; set; }
        public byte AlarmStata { get; set; }

        public ViewSources(byte[] bits)
        {

            TestFre = BitConverter.ToSingle(bits.Skip(0).Take(4).ToArray(), 0);
            TestCurrent = BitConverter.ToSingle(bits.Skip(4).Take(4).ToArray(), 0);
            TestPower = BitConverter.ToSingle(bits.Skip(8).Take(4).ToArray(), 0);
            TestIn = BitConverter.ToSingle(bits.Skip(12).Take(4).ToArray(), 0);
            TestIx1 = BitConverter.ToSingle(bits.Skip(16).Take(4).ToArray(), 0);
            TestIx2 = BitConverter.ToSingle(bits.Skip(20).Take(4).ToArray(), 0);
            TestIx3 = BitConverter.ToSingle(bits.Skip(24).Take(4).ToArray(), 0);
            TestIx4 = BitConverter.ToSingle(bits.Skip(28).Take(4).ToArray(), 0);
            TestPh1 = BitConverter.ToSingle(bits.Skip(32).Take(4).ToArray(), 0);
            TestPh2 = BitConverter.ToSingle(bits.Skip(36).Take(4).ToArray(), 0);
            TestPh3 = BitConverter.ToSingle(bits.Skip(40).Take(4).ToArray(), 0);
            TestPh4 = BitConverter.ToSingle(bits.Skip(44).Take(4).ToArray(), 0);
            TestRn = bits[48];
            TestRx1 = bits[49];
            TestRx2 = bits[50];
            TestRx3 = bits[51];
            TestRx4 = bits[52];
            TestSpeed = bits[53];
            OneVolate = BitConverter.ToSingle(bits.Skip(54).Take(4).ToArray(), 0);
            AlarmStata = bits[58];

            try
            {
                TestU0 = TestIn / (2 * Math.PI * TestFre * ((double)TestCn.value));

            }
            catch 
            {
            }
            //TestCx1 = TestIx1 / (2 * Math.PI * TestU0);
            //TestCx2 = TestIx2 / (2 * Math.PI * TestU0);
            //TestCx3 = TestIx3 / (2 * Math.PI * TestU0);
            //TestCx4 = TestIx4 / (2 * Math.PI * TestU0);
            //TestTan1 =Math.Tan(TestPh1 - Testθ0);
            //TestTan2 =Math.Tan(TestPh2 - Testθ0);
            //TestTan3 =Math.Tan(TestPh3 - Testθ0);
            //TestTan4 =Math.Tan(TestPh4 - Testθ0);
        }
        ~ViewSources()
        {
            // GC.Collect();
        }
    }
}
