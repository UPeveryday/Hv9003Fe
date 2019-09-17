using SCEEC.MI.High_Precision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCEEC.NET.TCPSERVER
{
    public delegate void SendTestResult(AsyncTCPServer tCPServer, byte[] testresult);
    public class TestMesseages
    {
        private double TestCn { get; set; }
        private double TestCnTan { get; set; }
        private bool IsRunning { get; set; }
        private bool IsSendData { get; set; }
        public ViewSources ViewData { get; set; }
        public bool ISStart = false;
        public event SendTestResult SendData;

        // public event EventArgs
        private AsyncTCPServer tCPServer;
        public byte[] data { get; set; }

        public byte[] testEnd { get; set; }
        public TestMesseages(AsyncTCPServer tCPServer, byte[] data)
        {
            this.tCPServer = tCPServer;
            this.data = data;
        }
        public TestMesseages()
        {

        }
        public void ReturnMessages(AsyncTCPServer AsTcp, byte[] data)
        {
            if (data != null)
            {
                switch (data[0])
                {
                    case 0x02:
                        TestClass.SerchSend(AsTcp, data);
                        break;
                    case 0xcc:
                        if (data[1] == 0x90)
                            TestClass.Connec(AsTcp, data);
                        else
                        {
                            if (MisTak.Success != TestResult.WorkTest.ChangeVolate(AnalysisData.DeelVolate(data)))
                                TestClass.SetPar(AsTcp, data, true);
                        }
                        break;
                    case 0xbc:
                        TestClass.DisConnec(AsTcp, data);
                        break;
                    case 0x41:
                        if (MisTak.Success == TestResult.WorkTest.ChangeFre(AnalysisData.DeelFre(data)))
                            TestClass.SetPar(AsTcp, data, true);
                        else
                            TestClass.SetPar(AsTcp, data, false);
                        break;
                    case 0x42:
                        if (MisTak.Success == TestResult.WorkTest.ChangeVolate(AnalysisData.DeelVolate(data)))//不确定是否是ASkll码
                            TestClass.SetPar(AsTcp, data, true);
                        else
                            TestClass.SetPar(AsTcp, data, false);
                        break;
                    case 0xac:
                        if (MisTak.Success == TestResult.WorkTest.StartPower())//无启动电源选项
                            TestClass.SetPar(AsTcp, data, true);
                        else
                            TestClass.SetPar(AsTcp, data, false);
                        break;
                    case 0xed:
                        if (MisTak.Success == TestResult.WorkTest.ClosePower())//无启动电源选项
                            TestClass.SetPar(AsTcp, data, true);
                        else
                            TestClass.SetPar(AsTcp, data, false);
                        break;
                    case 0x32:
                        TestResult.WorkTest.ChangeTestCn((float)Convert.ToDouble(AnalysisData.DeelCn(data)[0]));
                        TestResult.WorkTest.ChangeTestCnTan((float)Convert.ToDouble(AnalysisData.DeelCn(data)[1]));//CnTan协议无法测量
                        TestClass.SetPar(AsTcp, data, true);
                        break;
                    case 0x3a:
                        if (TestResult.WorkTest.IsTestting)//启动测量
                        {
                            IsRunning = true;
                            ISStart = true;
                        }
                        else
                        {
                            IsRunning = false;
                            ISStart = false;
                        }
                        TestClass.SetPar(AsTcp, data, ISStart);
                        break;
                    case 0xda:
                        // AnalysisData.DeelFreAndVolate(data);//发送需要的和中数据
                        TestClass.QueryFreAndVolate(AsTcp, data, new byte[36]);//电压频率,问题，高压侧电压低压侧电压
                        break;
                    case 0xff:
                        if (IsRunning)
                            TestClass.QueryTestState(AsTcp, data, new byte[] { 0xac, 0xac });
                        else
                            TestClass.QueryTestState(AsTcp, data, new byte[] { 0xee, 0xee });
                        break;
                    case 0xfd:
                        // TestClass.QueryTestResult(tCPServer, data, AnalysisData.DeelTestResult(TestResultData));//Test
                        ISStart = true;
                        SendData += TestMesseages_SendData;
                        break;
                    case 0xbd:
                        //反接板状态，查询协议确实
                        TestClass.QueryDisStata(AsTcp, data, true);
                        break;
                    default:
                        break;
                }
            }
        }
        public void ReturnMessages()
        {
            if (data != null)
            {
                switch (data[0])
                {
                    case 0x02:
                        TestClass.SerchSend(tCPServer, data);
                        break;
                    case 0xcc:
                        if (data[1] == 0x90)
                            TestClass.Connec(tCPServer, data);
                        else
                        {
                            if (MisTak.Success != TestResult.WorkTest.ChangeVolate(AnalysisData.DeelVolate(data)))
                                TestClass.SetPar(tCPServer, data, true);
                        }
                        break;
                    case 0xbc:
                        TestClass.DisConnec(tCPServer, data);
                        break;
                    case 0x41:
                        TestResult.WorkTest.ChangeFre(AnalysisData.DeelFre(data));
                        TestClass.SetPar(tCPServer, data, true);
                        break;
                    case 0x42:
                        TestResult.WorkTest.ChangeVolate(AnalysisData.DeelVolate(data));//不确定是否是ASkll码
                        TestClass.SetPar(tCPServer, data, true);
                        break;
                    case 0xac:
                        // TestResult.WorkTest.ChangeFre(1f);//无启动电源选项
                        TestClass.SetPar(tCPServer, data, true);
                        break;
                    case 0xed:
                        bool Istrue = false;
                        if (MisTak.Success == TestResult.WorkTest.startDownVolate())
                            Istrue = false;
                        else
                            Istrue = true;
                        TestClass.SetPar(tCPServer, data, Istrue);
                        break;
                    case 0x32:
                        TestResult.WorkTest.ChangeTestCn((float)Convert.ToDouble(AnalysisData.DeelCn(data)[0]));
                        TestResult.WorkTest.ChangeTestCn((float)Convert.ToDouble(AnalysisData.DeelCn(data)[1]));//CnTan协议无法测量
                        TestClass.SetPar(tCPServer, data, true);
                        break;
                    case 0x3a:
                        if (0x04 != TestResult.WorkTest.StartTest())//启动测量
                        {
                            IsRunning = true;
                            TestResult.WorkTest.OutTestResult += WorkTest_OutTestResult1;
                            ISStart = true;
                        }
                        else
                        {
                            IsRunning = false;
                            ISStart = false;
                        }
                        TestClass.SetPar(tCPServer, data, ISStart);
                        break;
                    case 0xda:
                        // AnalysisData.DeelFreAndVolate(data);//发送需要的和中数据
                        TestClass.QueryFreAndVolate(tCPServer, data, new byte[36]);//电压频率,问题，高压侧电压低压侧电压
                        break;
                    case 0xff:
                        if (IsRunning)
                            TestClass.QueryTestState(tCPServer, data, new byte[] { 0xac, 0xac });
                        else
                            TestClass.QueryTestState(tCPServer, data, new byte[] { 0xee, 0xee });
                        break;
                    case 0xfd:
                        // TestClass.QueryTestResult(tCPServer, data, AnalysisData.DeelTestResult(TestResultData));//Test
                        ISStart = true;
                        SendData += TestMesseages_SendData;
                        break;
                    case 0xbd:
                        //反接板状态，查询协议确实
                        TestClass.QueryDisStata(tCPServer, data, true);
                        break;
                    default:
                        break;
                }
            }
        }

        private void TestMesseages_SendData(AsyncTCPServer tCPServer, byte[] testresult)
        {
            TestClass.QueryTestResult(tCPServer, data, AnalysisData.DeelTestResult(testresult));
        }

        private void WorkTest_OutTestResult1(byte[] result)
        {
            if (ISStart)
            {
                SendData(tCPServer, result);
                ISStart = false;
            }
            // ViewSources vs = new ViewSources(result);
        }

        ~TestMesseages()
        {
            GC.Collect();
            // tCPServer.Dispose();
        }
    }

}
