using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SCEEC.MI.High_Precision
{
    public delegate void ResultDelegate(byte[] result);
    public enum TestKind
    {
        SetTestChannel,
        SetTestSpeed,
        SetTestCn,
        SetTestConfireVolate,
        SetTestConfireFre,
        StartBooster,
        StartBuck
    }


    public enum MisTak
    {
        Success = 0,
        InstructionFalse = 1,
        LengthFalse = 2,
        CheckFalse = 3,
        CommunicationFalse = 4
    }
    public class High_PrecisionClass
    {
        public SerialClass LocalPrecision = new SerialClass();
        public readonly string ComPort;
        public bool IsTestting = false;
        public double Cn { get; set; }
        public double CnTan { get; set; }
        public High_PrecisionClass()
        {
            string[] cp = GetPortNames();
            if (cp.Length == 1 && cp[0] == "COM4")
            {
                throw new Exception("未检测到可用端口");
            }
            else
            {
                List<string> Comp = new List<string>();
                foreach (string a in cp)
                {
                    if (a != "COM4")
                        Comp.Add(a);
                }
                string[] cpd = Comp.ToArray();
                this.ComPort = cpd[cpd.Length - 1];
                OpenPort();
            }
        }

        public void StopRecv(int time)
        {
            LocalPrecision.ReceiveEventFlag = true;
            Thread.Sleep(time);
            LocalPrecision.ReceiveEventFlag = false;
        }

        public void StopRe()
        {
            LocalPrecision.ReceiveEventFlag = true;

        }
        public void ContinuieRe()
        {
            LocalPrecision.ReceiveEventFlag = false;

        }
        public High_PrecisionClass(string comPortName)
        {
            this.ComPort = comPortName;
            LocalPrecision.setSerialPort(comPortName, 9600, 8, 1);

        }
        public bool OpenPort()
        {
            bool IsSuccess = true;
            try
            {
                LocalPrecision.setSerialPort(ComPort, 9600, 8, 1);
                LocalPrecision.openPort();
                //    LocalPrecision.DataReceived += new PortClass.SerialPortDataReceiveEventArgs(DataReceive);
                LocalPrecision.DataReceived += LocalPrecision_DataReceived;
            }
            catch (Exception)
            {
                return !IsSuccess;
            }
            return IsSuccess;


        }

        private void LocalPrecision_DataReceived(object sender, SerialDataReceivedEventArgs e, byte[] bits)
        {
            OutTestResult(bits);

        }

        private MisTak RetureFalse(byte wrong)
        {
            if (wrong == 0x01)
                return MisTak.Success;
            if (wrong == 0x02)
                return MisTak.InstructionFalse;
            if (wrong == 0x03)
                return MisTak.LengthFalse;
            if (wrong == 0x04)
                return MisTak.CheckFalse;
            else
                return MisTak.CommunicationFalse;
        }

        private byte CheckData(byte[] checkdata)
        {
            byte[] tempD = new byte[checkdata.Length];
            for (int i = 0; i < checkdata.Length; i++)
            {
                tempD[i] = checkdata[i];
            }
            // byte[] tempD = checkdata;

            byte Endcheckdata = 0;
            foreach (byte outd in tempD)
            {
                Endcheckdata += outd;
            }
            return Endcheckdata;
        }



        private bool IsCheckData(byte[] checkdata)
        {
            byte[] tempD = new byte[checkdata.Length - 1];
            for (int i = 0; i < checkdata.Length - 1; i++)
            {
                tempD[i] = checkdata[i];
            }
            byte Endcheckdata = 0;
            foreach (byte outd in tempD)
            {
                Endcheckdata += outd;
            }
            return Endcheckdata == checkdata[checkdata.Length - 1];
        }
        public string[] GetPortNames()
        {
            return LocalPrecision.getSerials();

        }
        public void Closeport()
        {
            LocalPrecision.closePort();
        }
        public event ResultDelegate OutTestResult;
        private static byte[] ReturnTestResult(byte[] bits)
        {
            return bits;
        }
        public void DataReceive(object sender, SerialDataReceivedEventArgs e, byte[] bits)
        {
            OutTestResult(bits);
        }

        public byte StartTest()
        {
            if (!IsTestting)
            {
                byte[] Issuccss = new byte[4096];
                byte[] TestComman = { 0x69, 0x6A, CheckData(new byte[2] { 0x69, 0x6A }) };
                LocalPrecision.SendCommand(TestComman, ref Issuccss, 10);
                if (Issuccss[0] == 0x01)
                {
                    IsTestting = true;
                    return Issuccss[0];
                }
            }
            return 0x04;
        }


        public MisTak ChangeTestChannel(byte testChannel)
        {
            byte[] Issuccss = new byte[4096];
            byte[] TestComman = { 0x80, testChannel, CheckData(new byte[2] { 0x80, testChannel }) };
            // LocalPrecision.SendDataByte(TestComman, 0, TestComman.Length);
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }
        public MisTak ChangeTestSpeed(byte testSpeed)
        {
            byte[] Issuccss = new byte[4096];
            byte[] TestComman = { 0x88, testSpeed, CheckData(new byte[2] { 0x88, testSpeed }) };
            // LocalPrecision.SendDataByte(TestComman, 0, TestComman.Length);
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }
        public MisTak ChangeTestCn(float testCn = 1e-12f)
        {
            byte[] testCnBuffer = BitConverter.GetBytes(testCn);
            byte[] Issuccss = new byte[4096];

            if (testCnBuffer.Length == 4)
            {
                byte[] TestComman = { 0x59, testCnBuffer[0], testCnBuffer[1], testCnBuffer[2], testCnBuffer[3],
                    CheckData(new byte[5] { 0x59, testCnBuffer[0], testCnBuffer[1], testCnBuffer[2], testCnBuffer[3] }) };
                if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                    return RetureFalse(Issuccss[0]);
                return RetureFalse(Issuccss[0]);
            }
            return RetureFalse(Issuccss[0]);

        }

        public MisTak ChangeTestCnTan(float CnTan = 0.01f)
        {
            byte[] testCnTanBuffer = BitConverter.GetBytes(CnTan);
            byte[] Issuccss = new byte[4096];

            if (testCnTanBuffer.Length == 4)
            {

                byte[] TestComman = { 0x5A, testCnTanBuffer[0], testCnTanBuffer[1], testCnTanBuffer[2], testCnTanBuffer[3],
                    CheckData(new byte[5] {0x5A, testCnTanBuffer[0], testCnTanBuffer[1], testCnTanBuffer[2], testCnTanBuffer[3]}) };
                if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                    return RetureFalse(Issuccss[0]);
                return RetureFalse(Issuccss[0]);
            }
            return RetureFalse(Issuccss[0]);
        }

        public MisTak ChangeVolate(float TestVolate, float volate2 = 500f)
        {
            byte[] testCnBuffer = BitConverter.GetBytes(TestVolate);
            byte[] changevolate = BitConverter.GetBytes(volate2);
            byte[] Issuccss = new byte[4096];
            if (testCnBuffer.Length == 4)
            {
                byte[] TestComman = { 0x50, testCnBuffer[0], testCnBuffer[1], testCnBuffer[2], testCnBuffer[3],
                    changevolate[0], changevolate[1], changevolate[2], changevolate[3],
                    CheckData(new byte[9] { 0x50, testCnBuffer[0], testCnBuffer[1], testCnBuffer[2], testCnBuffer[3],
                        changevolate[0], changevolate[1], changevolate[2], changevolate[3] }) };
                if (0 < LocalPrecision.SendCommandNotRecv(TestComman, ref Issuccss, 10))
                    return RetureFalse(Issuccss[0]);
                return RetureFalse(Issuccss[0]);
            }
            return RetureFalse(Issuccss[0]);
        }

        public MisTak ChangeFre(float TestFre)
        {
            byte[] Issuccss = new byte[4096];
            byte[] testCnBuffer = BitConverter.GetBytes(TestFre);
            if (testCnBuffer.Length == 4)
            {
                byte[] TestComman = { 0x55, testCnBuffer[0], testCnBuffer[1], testCnBuffer[2], testCnBuffer[3],
                    CheckData(new byte[5] { 0x55, testCnBuffer[0], testCnBuffer[1], testCnBuffer[2], testCnBuffer[3] }) };
                if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                    return RetureFalse(Issuccss[0]);
                return RetureFalse(Issuccss[0]);
            }
            return RetureFalse(Issuccss[0]);
        }

        public void BoomFive()
        {
            byte[] tpd = { 0x65, 0x65, CheckData(new byte[] { 0x65, 0x65 }) };
            LocalPrecision.SendDataByte(tpd, 0, 3);
        }

        public void Reset()
        {
            byte[] tpd = { 0x31, 0x31, 0x62 };
            LocalPrecision.SendDataByte(tpd, 0, 3);
        }

        public MisTak startUpVolate()
        {
            byte[] Issuccss = new byte[4096];
            byte[] TestComman = { 0xA0, 0xA0, CheckData(new byte[2] { 0xA0, 0xA0 }) };
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }

        public MisTak startDownVolate()
        {
            byte[] Issuccss = new byte[4096];
            byte[] TestComman = { 0xAA, 0xAA, CheckData(new byte[2] { 0xAA, 0xAA }) };
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }


        public MisTak ClearAlarm()
        {
            byte[] Issuccss = new byte[4096];
            byte[] TestComman = { 0x31, 0x31, CheckData(new byte[2] { 0x31, 0x31 }) };
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }

        public MisTak StartPower()
        {
            byte[] Issuccss = new byte[4096];
            byte[] TestComman = { 0x41, 0x41, CheckData(new byte[2] { 0x41, 0x41 }) };
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }


        public MisTak ClosePower()
        {
            byte[] Issuccss = new byte[4096];
            byte[] TestComman = { 0x51, 0x51, CheckData(new byte[2] { 0x51, 0x51 }) };
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }
        /// <summary>
        /// 修正参数
        /// </summary>
        /// <param name="Channel">通道数</param>
        /// <param name="ExtensionNum">扩展板数</param>
        /// <param name="Range">量程</param>
        /// <param name="AmplitudeNum">幅度修正值</param>
        /// <param name="TanNum">角度修正值</param>
        /// <returns></returns>
        public MisTak CorrectionPra(byte Channel, byte ExtensionNum, byte Range, float AmplitudeNum, float TanNum)
        {
            byte[] Issuccss = new byte[4096];
            byte[] At = BitConverter.GetBytes(AmplitudeNum);
            byte[] tn = BitConverter.GetBytes(TanNum);
            byte[] TestComman = { 0x81, Channel, ExtensionNum, Range, At[0], At[1], At[2], At[3], tn[0], tn[1], tn[2], tn[3],
                CheckData(new byte[12] { 0x81, Channel, ExtensionNum, Range, At[0], At[1], At[2], At[3], tn[0], tn[1], tn[2], tn[3] }) };
            if (0 < LocalPrecision.SendCommand(TestComman, ref Issuccss, 10))
                return RetureFalse(Issuccss[0]);
            return RetureFalse(Issuccss[0]);
        }
        private byte[] FloatBuffreToByteBuffer(float[] buf)
        {
            List<byte> ret1 = new List<byte>();
            foreach (var a in buf)
            {
                byte[] tp = BitConverter.GetBytes(a);
                for (int i = 0; i < 4; i++)
                {
                    ret1.Add(tp[i]);
                }
            }

            return ret1.ToArray();
        }

        public void Sendlargedata(float[] data)
        {
            LocalPrecision.SendDataByte(FloatBuffreToByteBuffer(data), 0, data.Length * 4);
        }

        public float[] ReadCheckPra()
        {
            byte[] Issuccss = new byte[3584];
            float[] result = new float[896];
            byte[] TestComman = { 0x82, 0x82, 0x04 };
            byte[] ts = LocalPrecision.ReadPortsData(TestComman, Issuccss, 3584, 50);
            if (ts.Length == 3584)
            {
                for (int i = 0; i < 896; i++)
                {
                    result[i] = BitConverter.ToSingle(ts, i * 4);
                }
                return result;
            }
            else
            {
                return null;
            }
        }





        public void ModifyMeasurementParameters(byte TestChannel, byte TestSpeed, byte Cn, byte Volate, byte Fre, TestKind kind)
        {
            if (kind == TestKind.SetTestChannel)
                ChangeTestChannel(TestChannel);
            if (kind == TestKind.SetTestSpeed)
                ChangeTestSpeed(TestSpeed);
            if (kind == TestKind.SetTestCn)
                ChangeTestCn(Cn);
            if (kind == TestKind.SetTestConfireVolate)
                ChangeVolate(Volate);
            if (kind == TestKind.SetTestConfireFre)
                ChangeFre(Fre);
            if (kind == TestKind.StartBooster)
                startUpVolate();
            if (kind == TestKind.StartBuck)
                startDownVolate();
        }


        ~High_PrecisionClass()
        {
            //GC.Collect();
            Closeport();
        }
    }

    public static class TestResult
    {
        public static High_PrecisionClass WorkTest = new High_PrecisionClass();
    }


}
