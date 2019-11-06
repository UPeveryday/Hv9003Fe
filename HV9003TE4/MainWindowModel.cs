using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCEEC.Numerics;
using SCEEC.MI.High_Precision;
using SCEEC.NET.TCPSERVER;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading;
using System.Windows;
using LiveCharts.Defaults;
using System.Collections.ObjectModel;
using HV9003TE4.Models;
using System.Drawing;
using System.Diagnostics;

namespace HV9003TE4
{
    public delegate void ReturnResult(byte[] data);

    public class MainWindowModel : INotifyPropertyChanged
    {
        TestMesseages TestMesseagesNull = new TestMesseages();
        public volatile byte[] TestRes;
        public List<string> TaskList { get; set; }
        public bool IsEnable { get; set; } = false;
        public string YPopu { get; set; }
        public bool ISREMOTE { get; set; } = true;
        public bool ISCLOSE { get; set; } = true;
        public bool VolateState { get; set; } = false;
        public bool ISZERO { get; set; } = true;
        public ObservableCollection<string> ListboxItemsources { get; set; } = new ObservableCollection<string>();
        public Models.SysAutoTestResult MyTestList { get; set; }
        public Visibility Vs { get; set; } = Visibility.Visible;
        public ObservableCollection<string> TestResultMeassge { get; set; } = new ObservableCollection<string>();//测量过程信息
        public string Process { get; set; }//
        public int ProcesNum { get; set; }
        public ushort FontSize { get; set; } = 15;

        public double VolateSpeed { get; set; } = 2;
        public ObservableCollection<string> projiectnames { get; set; } = new ObservableCollection<string>();//方案列表

        public ObservableCollection<string> ProjectVolate { get; set; } = new ObservableCollection<string>();//添加的方案数据
        public List<int> TimeSources { get; set; } = new List<int>();
        public bool IsCompleteVolateTest { get; set; } = true;
        public Visibility QuqlityIsOk { get; set; } = Visibility.Collapsed;
        public ObservableCollection<UIElement> Projectdataui { get; set; }

        public double YMaxValue { get; set; }

        public PhysicalVariable SourceFrequency { get; set; } = "50.0 Hz";
        public PhysicalVariable SourceVoltage { get; set; } = "100.0 V";
        public PhysicalVariable SourceCurrent { get; set; } = "10 A";
        public PhysicalVariable SourcePower { get; set; } = "1.000 kW";
        public PhysicalVariable HVFrequency { get; set; } = "50.0 Hz";
        public PhysicalVariable HVVoltage { get; set; } = "100.0 kV";
        public PhysicalVariable Cn { get; set; } = "99.868pF";
        public PhysicalVariable In { get; set; } = "10.00 uA";
        public PhysicalVariable AGn { get; set; } = "0.000001";
        public PhysicalVariable Ix1 { get; set; } = "2.000 mA";
        public PhysicalVariable AG1 { get; set; } = "0.000001";
        public PhysicalVariable Ix2 { get; set; } = "2.000 mA";
        public PhysicalVariable AG2 { get; set; } = "0.000002";
        public PhysicalVariable Ix3 { get; set; } = "2.000 mA";
        public PhysicalVariable AG3 { get; set; } = "0.000003";
        public PhysicalVariable Ix4 { get; set; } = "2.000 mA";
        public PhysicalVariable AG4 { get; set; } = "0.000004";


        public void StartTcp()
        {
            TcpTask.TcpServer.DataReceived += TcpServer_DataReceived;
            TcpTask.TcpServer.ClientConnected += TcpServer_ClientConnected;
            TcpTask.TcpServer.ClientDisconnected += TcpServer_ClientDisconnected;
            TcpTask.TcpServer.CompletedSend += TcpServer_CompletedSend;
        }

        private void TcpServer_CompletedSend(object sender, AsyncEventArgs e)
        {

        }

        private void TcpServer_ClientDisconnected(object sender, AsyncEventArgs e)
        {
            WindowIsEnable = false;
            ConnectStateControl = 0.4f;
        }

        private void TcpServer_ClientConnected(object sender, AsyncEventArgs e)
        {
            ConnectStateControl = 1f;
        }
        public float ConnectStateControl { get; set; } = 0.6f;

        public bool Pane1Enable { get; set; } = false;
        public bool Pane2Enable { get; set; } = false;
        public bool Pane3Enable { get; set; } = false;
        public bool Pane4Enable { get; set; } = false;
        public bool AllAutoTestIsOpen { get; set; } = false;

        public void OpenNewTestWindow()
        {
            while (AllAutoTestIsOpen == true)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    new Views.AllAutoTest().ShowDialog();
                    AllAutoTestIsOpen = false;
                }));
            }
        }
        public event AllAutoTestFlag OpenAutoTest;
        public bool MyPAllAutoTestOrOrdeTestroperty { get; set; } = false;
        private void TcpServer_DataReceived(object sender, AsyncEventArgs e)
        {

            byte[] a = e._state.Buffer;
            int length = e._state.RecLength;
            var Temp = a.Skip(0).Take(length).ToArray();
            ControlRemoteState(Temp);
            if (Temp[0] == 0xfd)
                TestClass.QueryTestResult(TcpTask.TcpServer, null, AnalysisData.DeelTestResult(TestRes));
            if (Temp[0] == 0xda)
            {
                TestClass.QueryTestResult(TcpTask.TcpServer, null, AnalysisData.DeelVolateAndFre(TestRes));
            }
            if (!MyPAllAutoTestOrOrdeTestroperty)
            {
                if (Temp[0] == 0xdd && Temp[1] == 0x0a)
                {
                    StartPower();
                    Thread.Sleep(500);
                    byte[] array1 = System.Text.Encoding.ASCII.GetBytes("OK");
                    TestClass.QueryTestResult(TcpTask.TcpServer, null, array1);
                    Models.StaticClass.IsTcpTestting = true;
                    SysData = Temp;
                    try
                    {
                        if (Temp[2] == 0x01) StaticClass.AllTestResult.PanelEnable = true; else StaticClass.AllTestResult.PanelEnable = false;
                        if (Temp[3] == 0x01) StaticClass.AllTestResult.Pane2Enable = true; else StaticClass.AllTestResult.Pane2Enable = false;
                        if (Temp[4] == 0x01) StaticClass.AllTestResult.Pane3Enable = true; else StaticClass.AllTestResult.Pane3Enable = false;
                        if (Temp[5] == 0x01) StaticClass.AllTestResult.Pane4Enable = true; else StaticClass.AllTestResult.Pane4Enable = false;
                    }
                    catch { }
                    //FileListItems(Temp);
                    // StartAutobytcp();
                    MyPAllAutoTestOrOrdeTestroperty = true;//可以删除
                    OpenAutoTest(Temp);
                }
            }
            if (MyPAllAutoTestOrOrdeTestroperty)
            {
                if (Temp[0] == 0xdd && Temp[1] == 0x0b)
                {
                    if (Models.StaticClass.IsTcpTestting)
                    {
                        byte[] array = System.Text.Encoding.ASCII.GetBytes("BUSY");
                        TestClass.QueryTestResult(TcpTask.TcpServer, null, array);
                    }
                    else
                    {

                        TestClass.QueryTestResult(TcpTask.TcpServer, null, StaticClass.Getbytesdata(StaticClass.AllTestResult, 2));
                    }
                }
            }
            if (Temp[0] == 0xdd && Temp[1] == 0x00 && Temp[2] == 0x01)
            {
                byte[] array1 = System.Text.Encoding.ASCII.GetBytes("OK");
                TestClass.QueryTestResult(TcpTask.TcpServer, null, array1);
            }

            if (Temp[0] == 0xdd && Temp[1] == 0xff)
            {
                WindowIsEnable = false;
            }

        }

        public bool WindowIsEnable { get; set; } = true;

        public bool IsSavedWaveImage { get; set; } = false;

        private void SetVolate(float volate)
        {
            SetBaseVolate(volate, (float)VolateSpeed);
            Thread.Sleep(300);
            UpVolate();

        }
        #region AUTO
        public bool ISELEY { get; set; } = false;
        public bool ISELEVOLATE { get; set; } = false;
        public bool TestByProject { get; set; } = false;

        public DateTime StartTime { get; set; }

        //  public FourTestResult AllTestResult { get; set; } = new FourTestResult();
        public void StartAutoTestAsync(Models.SysAutoTestResult sys)
        {
            StartTime = DateTime.Now;
            DatagridData = new ObservableCollection<TestResultDataGrid>(); //创建介损的datagrid
            if (Models.StaticClass.IsTcpTestting)
                sys = StaticClass.GetDataForTcpAutoTest(SysData);
            #region 电压
            Models.AutoStateStatic.SState.Clear();
            float[] needtest = sys.NeedTestList.ToArray();
            StaticClass.AllTestResult.NeedTestNum = needtest.Length;
            for (int i = 0; i < sys.NeedTestList.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                resetEvent.WaitOne();
                AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
                SetVolate(needtest[i]);
                IsEnable = false;
                if (UpvolateIsOk())
                {
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await Task.Delay(2000);
                        DatagridData.Add(new TestResultDataGrid
                        {
                            volate = HVVoltage,
                            captance1 = Capacitance1,
                            captance2 = Capacitance2,
                            captance3 = Capacitance3,
                            captance4 = Capacitance4,
                            tan1 = DissipationFactor1,
                            tan2 = DissipationFactor2,
                            tan3 = DissipationFactor3,
                            tan4 = DissipationFactor4,
                            Qultitly = "升压成功"
                        });

                        AddFourPanelData(ChartPannel.Channel1, (double)HVVoltage.value, (double)NumericsConverter.Text2Value(DissipationFactor1).value);
                        AddFourPanelData(ChartPannel.Channel2, (double)HVVoltage.value, (double)NumericsConverter.Text2Value(DissipationFactor2).value);
                        AddFourPanelData(ChartPannel.Channel3, (double)HVVoltage.value, (double)NumericsConverter.Text2Value(DissipationFactor3).value);
                        AddFourPanelData(ChartPannel.Channel4, (double)HVVoltage.value, (double)NumericsConverter.Text2Value(DissipationFactor4).value);
                        AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
                    });
                    StaticClass.AllTestResult = StaticClass.ReturntestTestData(StaticClass.AllTestResult, Capacitance1, Capacitance2, Capacitance3, Capacitance4, DissipationFactor1, DissipationFactor2,
                          DissipationFactor3, DissipationFactor4, true);
                }
            }
            AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
            #endregion
            DownVolate();
            if (token.IsCancellationRequested)
            {
                return;
            }
            resetEvent.WaitOne();

            if (UpvolateIsOk())
            {
                if (sys.IsEleY)
                {
                    Thread.Sleep(3000);
                    if (MessageBox.Show("是否开始电晕实验？", "通知", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        StartEleY();
                    else
                    {
                        if (sys.IsVolate)
                        {
                            if (MessageBox.Show("是否开始耐压实验？", "通知", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                                StartVolate();
                        }
                    }
                }
            }
        }
        private void StartTestTask()
        {
            StartAutoTestAsync(GetSys());
        }

        public byte[] SysData { get; set; }
        private void StartTestTaskByTcp()
        {
            StartAutoTestAsync(StaticClass.GetDataForTcpAutoTest(SysData));
        }
        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        static CancellationToken token = tokenSource.Token;
        static ManualResetEvent resetEvent = new ManualResetEvent(true);
        public Task task { get; set; }
        public Task taskTcp { get; set; }
        public Task taskProject { get; set; }
        public SysAutoTestResult SysProject { get; set; }


        public void InitTest()
        {
            panel1SeriesCollection[0].Values.Clear();
            panel2SeriesCollection[0].Values.Clear();
            panel3SeriesCollection[0].Values.Clear();
            panel4SeriesCollection[0].Values.Clear();
            TanEleVolate[0].Values.Clear();
            //  DatagridData.Clear();
        }
        public void StartAuto()
        {
            task = new Task(StartTestTask, token);
            task.Start();
        }
        public void StartAutobytcp()
        {
            taskTcp = new Task(StartTestTaskByTcp, token);
            taskTcp.Start();
        }
        public void StartAutobyProject()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            resetEvent = new ManualResetEvent(true);
            taskProject = new Task(StartTestTaskByProject, token);
            //if (taskProject.Status != TaskStatus.Created)
            //    taskProject.Status = TaskStatus.Created
            taskProject.Start();

        }
        private void StartTestTaskByProject()
        {
            StartAutoTestAsync(SysProject);
        }
        public void ResetTest()
        {
            resetEvent.Reset();

        }

        public void CancerTest()
        {
            tokenSource.Cancel();
        }
        public void ContinuTest()
        {
            resetEvent.Set();
        }
        public volatile bool IsStartEleY = false;
        public bool UpvolateIsOk()
        {
            int p = 0;
            bool IsEnd = false;
            IsEnable = false;
            Thread.Sleep(300);
            while (!IsEnable)
            {
                p++;
                Thread.Sleep(1000);
                if (p > 20)
                {
                    IsEnd = true;
                    return false;
                }
                Thread.Sleep(1000);
            }
            if (!IsEnd)
            {
                return true;
            }
            else
                return false;
        }

        public void StartEleY()
        {
            Models.SysAutoTestResult sys = new SysAutoTestResult();
            if (Models.StaticClass.IsTcpTestting)
                sys = StaticClass.GetDataForTcpAutoTest(SysData);
            else
            {
                sys = GetSys();
            }
            Thread.Sleep(8000);
            SetVolate(sys.EleY);
            if (UpvolateIsOk())
            {
                QuqlityIsOk = Visibility.Visible;
                AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
                Thread.Sleep(2000);
                AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
                while (true)
                {
                    if (AutoStateStatic.SState.IsPress)
                    {
                        if (MessageBox.Show("是否开始耐压实验？", "通知", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            StartVolate();
                        }
                        else
                            AutoStateStatic.SState.IsStartVolate = false;
                        AutoStateStatic.SState.IsPress = false;
                        break;
                    }
                }

            }

        }
        public void StartVolate()
        {
            Models.SysAutoTestResult sys = new SysAutoTestResult();
            if (Models.StaticClass.IsTcpTestting)
                sys = StaticClass.GetDataForTcpAutoTest(SysData);
            else
            {
                sys = GetSys();
            }
            SetVolate(sys.EleVolate);
            if (UpvolateIsOk())
            {
                AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
                for (int i = 0; i < sys.HideTime; i++)
                {
                    Thread.Sleep(1000);
                }
                AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
                SetVolate(0);
                Thread.Sleep(100);
                if (UpvolateIsOk())
                    AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
                #region 
            }
            #endregion
        }

        //public void StartVolate()
        //{
        //    Models.SysAutoTestResult sys = new SysAutoTestResult();
        //    if (Models.StaticClass.IsTcpTestting)
        //        sys = StaticClass.GetDataForTcpAutoTest(SysData);
        //    else
        //    {
        //        sys = GetSys();
        //    }
        //    Thread.Sleep(3000);
        //    SetVolate(sys.EleVolate);
        //    if (UpvolateIsOk())
        //    {
        //        AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
        //        Application.Current.Dispatcher.Invoke(async () =>
        //        {
        //            //AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
        //            for (int i = 0; i < sys.HideTime; i++)
        //            {
        //                await Task.Delay(1000);
        //            }
        //            AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
        //            SetVolate(0);
        //            bool IsEnd = false;
        //            for (int i = 0; i < 20; i++)
        //            {
        //                if (IsEnable != true)
        //                {
        //                    await Task.Delay(1000);
        //                    //  Thread.Sleep(1000);

        //                    IsEnd = true;
        //                }
        //                else
        //                {
        //                    IsEnd = false;
        //                    break;
        //                }
        //            }
        //            if (!IsEnd)
        //            {
        //                await Task.Delay(10000);
        //                AddTanEleVolatepoint((DateTime.Now - StartTime).TotalSeconds, (double)HVVoltage.value);
        //            }
        //        });


        //        #region 
        //    }
        //    #endregion
        //}
        private Models.SysAutoTestResult GetSys()
        {
            //  Models.AutoStateStatic.SState.Clear();
            Models.SysAutoTestResult sys = new Models.SysAutoTestResult();
            var tpd = ListboxItemsources.ToArray();
            for (int i = 0; i < tpd.Length - 2; i++)
            {
                string[] Usedata = tpd[i].Split(':');
                if (Usedata.Length == 2)
                {
                    //  Usedata[i] = Usedata[i].Trim();
                    sys.NeedTestList.Add((float)NumericsConverter.Text2Value(StaticClass.DeleteSpace(Usedata[1])).value);
                }

            }
            string[] Usedata1 = tpd[tpd.Length - 2].Split(':');
            sys.IsEleY = true;
            sys.IsVolate = true;
            sys.EleY = (float)NumericsConverter.Text2Value(StaticClass.DeleteSpace(Usedata1[1])).value;
            string[] p = tpd[tpd.Length - 1].Split(':');
            sys.EleVolate = (float)NumericsConverter.Text2Value(StaticClass.DeleteSpace(p[3])).value;
            try
            {
                sys.HideTime = Convert.ToInt32(StaticClass.DeleteSpace(p[1]));
            }
            catch
            {
                sys.HideTime = 60;
                ShowHide("键入的耐压保持时间格式错误" + "\t\n" + "以设置为默认60S");

            }
            return sys;
        }
        #endregion
        /// <summary>
        /// 远程状态
        /// </summary>
        /// <param name="temp"></param>
        private void ControlRemoteState(byte[] temp)
        {
            if (temp[0] == 0xdd)
            {
                if (temp[1] == 0x00)
                {
                    if (temp[2] == 0x01)
                    {
                        ISREMOTE = false;
                        IsEnable = false;
                        ISCLOSE = true;
                    }
                    if (temp[2] == 0x02)
                    {
                        ISREMOTE = true;
                        IsEnable = true;
                        ISCLOSE = false;

                    }
                }
                if (temp[1] == 0x03)
                {
                    try
                    {
                        string path = "WaveImage\\" + temp[2].ToString() + ".jpg";
                        TestClass.QueryTestResult(TcpTask.TcpServer, null, Models.StaticClass.getImageByte(path));
                    }
                    catch
                    {
                        ShowHide("未截取波形图\r\n" + "或者读取出错");
                    }
                }
            }
        }
        public void StartRecCom()
        {
            TestResult.WorkTest.StartTest();
            TestResult.WorkTest.OutTestResult += WorkTest_OutTestResult;
        }
        public void StartRecCom1()
        {
            TestResult.WorkTest.IsTestting = false;
            TestResult.WorkTest.StartTest();
            TestResult.WorkTest.OutTestResult += WorkTest_OutTestResult;
        }
        public void REset()
        {
            try
            {
                TestResult.WorkTest.Reset();
            }
            catch
            {
                ShowHide("复位失败\t\n请检查串口及仪器");
            }
        }


        public double pnv(double? a)
        {
            return (a is null) ? 0 : ((double)a);
        }
        private PhysicalVariable calcCap(PhysicalVariable Ix, PhysicalVariable AGx)
        {
            double angle = pnv(AGx.value);
            double ratio = pnv(Ix.value) / pnv(In.value);
            double zn = pnv(Cn.value) / Math.Cos(pnv(AGn.value));
            double z = zn * ratio;
            double cp = z * Math.Cos(angle);
            double rp = z * Math.Sin(angle);
            double tan = Math.Tan(pnv(AGx.value) - pnv(AGn.value));
            double cs = cp * (1 + tan * tan);
            if ((cs < 1e24) && (cs > -1e24))
                return NumericsConverter.Value2Text(cs, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Capacitance);
            else
                return "NaN";

        }
        private string calcDF(PhysicalVariable AGx)
        {
            double tan = Math.Tan(pnv(AGx.value) + pnv(AGn.value));
            if ((tan < 1e24) && (tan > -1e24))
                return NumericsConverter.Value2Text(tan, 4, -5, "", "", percentage: true, usePrefix: false).Trim();
            else
                return "NaN";

        }
        private string calcPower(PhysicalVariable Ix, PhysicalVariable AGx)
        {
            double angle = pnv(AGx.value);
            double ratio = pnv(Ix.value) / pnv(In.value);
            double zn = pnv(Cn.value) / Math.Cos(pnv(AGn.value));
            double z = zn * ratio;
            double cp = z * Math.Cos(angle);
            double rp = z * Math.Sin(angle);
            double tan = Math.Tan(pnv(AGx.value) - pnv(AGn.value));
            double rs = cp * (tan * tan) / (1 + tan * tan);
            double i = pnv(Ix.value);
            double p = i * i * rs;
            if ((p < 1e24) && (p > -1e24))
                return NumericsConverter.Value2Text(p, 4, -3, "", SCEEC.Numerics.Quantities.QuantityName.Power);
            else
                return "NaN";
        }
        private string calcVolt(PhysicalVariable Freq, PhysicalVariable C, PhysicalVariable I)
        {
            double freq = pnv(Freq.value);
            double c = pnv(C.value);
            double i = pnv(I.value);
            if (freq < 1.0) return "NaN";
            if (c < 1e-13) return "NaN";
            if (i < 1e-9) return "0 V";
            double v = i / (2 * Math.PI * freq * c);
            return NumericsConverter.Value2Text(v, 4, -1, "", SCEEC.Numerics.Quantities.QuantityName.Voltage);
        }
        public string StandardCapacitance { get; set; }
        public string StandardCapacitanceDissipationFactor { get; set; }
        public string Capacitance1 { get; set; }
        public string Capacitance2 { get; set; }
        public string Capacitance3 { get; set; }
        public string Capacitance4 { get; set; }
        public string Current1 { get; set; }
        public string Current2 { get; set; }
        public string Current3 { get; set; }
        public string Current4 { get; set; }
        public string DissipationFactor1 { get; set; }
        public string DissipationFactor2 { get; set; }
        public string DissipationFactor3 { get; set; }
        public string DissipationFactor4 { get; set; }
        public string Power1 { get; set; }
        public string Power2 { get; set; }
        public string Power3 { get; set; }
        public string Power4 { get; set; }
        public string Alarm { get; set; }
        public string OneStata { get; set; }
        private double TestFre;
        public float Volate { get; set; }
        private void WorkTest_OutTestResult(byte[] result)
        {
            AlarmLock(result);
            if (result[58] == 0x09 || result[58] == 0x00)
            {
                ViewSources vs = new ViewSources(result);
                this.TestFre = vs.TestFre;
                this.In = NumericsConverter.Value2Text(vs.TestIn, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Capacitance);
                this.Ix1 = NumericsConverter.Value2Text(vs.TestIx1, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Current);
                this.AG1 = vs.TestPh1.ToString("F6"); //NumericsConverter.Value2Text(vs.TestPh1, 4, -6, "", SCEEC.Numerics.Quantities.QuantityName.None);
                this.Ix2 = NumericsConverter.Value2Text(vs.TestIx2, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Current);
                this.AG2 = vs.TestPh2.ToString("F6"); //NumericsConverter.Value2Text(vs.TestPh2, 4, -6, "", SCEEC.Numerics.Quantities.QuantityName.None);
                this.Ix3 = NumericsConverter.Value2Text(vs.TestIx3, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Current);
                this.AG3 = vs.TestPh3.ToString("F6"); //NumericsConverter.Value2Text(vs.TestPh3, 4, -6, "", SCEEC.Numerics.Quantities.QuantityName.None);
                this.Ix4 = NumericsConverter.Value2Text(vs.TestIx4, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Current);
                this.AG4 = vs.TestPh4.ToString("F6"); //NumericsConverter.Value2Text(vs.TestPh4, 4, -6, "", SCEEC.Numerics.Quantities.QuantityName.None);
                this.OneStata = NumericsConverter.Value2Text(vs.OneVolate, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage);
                this.Alarm = vs.AlarmStata.ToString();
                this.Volate = (float)vs.OneVolate;
                SourceFrequency = NumericsConverter.Value2Text(vs.TestFre, 4, -3, "", SCEEC.Numerics.Quantities.QuantityName.Frequency);
                SourceCurrent = NumericsConverter.Value2Text(vs.TestCurrent * 10, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Current);
                SourcePower = NumericsConverter.Value2Text(vs.TestPower, 4, 0, "", SCEEC.Numerics.Quantities.QuantityName.Power);
                SourceVoltage = OneStata;
                StandardCapacitance = Cn.ToString();
                StandardCapacitanceDissipationFactor = NumericsConverter.Value2Text(Math.Tan(pnv(AGn.value)), 4, -5, "", SCEEC.Numerics.Quantities.QuantityName.None, percentage: true).Trim();
                HVFrequency = NumericsConverter.Value2Text(vs.TestFre, 4, -3, "", SCEEC.Numerics.Quantities.QuantityName.Frequency);
                HVVoltage = calcVolt(HVFrequency, Cn, In);
                Capacitance1 = calcCap(Ix1, AG1).ToString();
                Capacitance2 = calcCap(Ix2, AG2).ToString();
                Capacitance3 = calcCap(Ix3, AG3).ToString();
                Capacitance4 = calcCap(Ix4, AG4).ToString();
                Current1 = Ix1.ToString();
                Current2 = Ix2.ToString();
                Current3 = Ix3.ToString();
                Current4 = Ix4.ToString();
                DissipationFactor1 = calcDF(AG1);
                DissipationFactor2 = calcDF(AG2);
                DissipationFactor3 = calcDF(AG3);
                DissipationFactor4 = calcDF(AG4);
                Power1 = calcPower(Ix1, AG1);
                Power2 = calcPower(Ix2, AG2);
                Power3 = calcPower(Ix3, AG3);
                Power4 = calcPower(Ix4, AG4);
                TestRes = result;
                if (result[58] == 9)
                {
                    IsEnable = false;//对升压状态的处理
                }
                else if (result[58] == 0)
                {
                    IsEnable = true;
                }
            }
        }
        private void AlarmLock(byte[] data)
        {

            if (data[58] == 1)
            {
                MessageBox.Show("升压失败", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = true;
                REset();
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = false;
            }
            if (data[58] == 2)
            {
                MessageBox.Show("变频电源通讯失败", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = true;
                REset();
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = false;

            }
            if (data[58] == 3)
            {
                MessageBox.Show("变频电源过流", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = true;
                REset();
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = false;
            }
            if (data[58] == 4)
            {
                MessageBox.Show("变频电源开启或者输出失败", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = true;
                REset();
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = false;
            }
            if (data[58] == 6)
            {
                MessageBox.Show("标容侧无信号", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = true;
                REset();
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = false;

            }
            if (data[58] == 7)
            {
                MessageBox.Show("被试侧过流", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = true;
                REset();
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = false;
            }
            if (data[58] == 8)
            {
                MessageBox.Show("心跳丢失", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = true;
                REset();
                TestResult.WorkTest.LocalPrecision.ReceiveEventFlag = false;
            }

            Thread.Sleep(1000);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #region Command



        // public ICommand ReadMultPra { get { return new RelayCommand(ReadMult, CanReadMult); } }

        public bool CanReadMult()
        {
            return true;
        }

        public float[] ReadMult()
        {
            return TestResult.WorkTest.ReadCheckPra();
        }

        public void SendLargeData(float[] data)
        {
            foreach (var a in data)
            {
                TestResult.WorkTest.Sendlargedata(data);
            }
        }
        #endregion
        #region
        public void StartPower()
        {
            try
            {
                TestResult.WorkTest.StartPower();
                VolateState = true;
                //TestClass.QueryTestResult(TcpTask.TcpServer, null, new byte[3] { 0xdd, 0x01, 0x01 });
            }
            catch
            {
                VolateState = false;
                TestClass.QueryTestResult(TcpTask.TcpServer, null, new byte[3] { 0xdd, 0x01, 0x02 });
                //ShowHide("开启电源失败" + "\r\n" + "请检查串口和电源");
            }
        }
        public void ClosePower()
        {
            try
            {
                TestResult.WorkTest.ClosePower();
                TestClass.QueryTestResult(TcpTask.TcpServer, null, new byte[3] { 0xdd, 0x01, 0x02 });
                VolateState = false;

            }
            catch
            {
                VolateState = true;
                TestClass.QueryTestResult(TcpTask.TcpServer, null, new byte[3] { 0xdd, 0x01, 0x01 });
                ShowHide("关闭电源失败" + "\r\n" + "请检查串口和电源");
            }

        }

        public void UpVolate()
        {
            try
            {

                TestResult.WorkTest.StopRe();
                IsEnable = false;
                TestResult.WorkTest.startUpVolate();
                IsEnable = false;
                Thread.Sleep(3000);
                TestResult.WorkTest.ContinuieRe();
            }
            catch
            {
                ShowHide("升压失败" + "\r\n" + "请检查串口和电源");
            }
        }
        public void ShowHide(string Text)
        {
            Views.Alarm alarm = new Views.Alarm(Text);
            alarm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            alarm.ShowDialog();
        }
        public void DownVolate()
        {
            try
            {
                ISZERO = false;
                TestClass.SendData(new byte[3] { 0xdd, 0x04, 0x01 }, TcpTask.TcpServer);
                TestResult.WorkTest.startDownVolate();
                TestClass.QueryTestResult(TcpTask.TcpServer, null, new byte[3] { 0xdd, 0x04, 0x02 });
                ISZERO = true;
            }
            catch
            {
                ISZERO = true;
                ShowHide("降压失败" + "\r\n" + "请检查串口和电源");
            }
        }
        public void SetBaseVolate(float vol, float volatespeed)
        {
            try
            {

                TestResult.WorkTest.ChangeVolate(vol, volatespeed * 1000);

            }
            catch
            {
                ShowHide("设置基础电压失败" + "\r\n" + "请检查串口和电源");
            }
        }
        public void AddVolate(float vol)
        {
            TestResult.WorkTest.ChangeVolate(vol);
        }
        public void MulVolate(float vol)
        {
            TestResult.WorkTest.ChangeVolate(vol);
        }

        public void SetFre(float fre)
        {
            TestResult.WorkTest.ChangeFre(fre);

        }
        public void AddFre(double fre)
        {
            try
            {
                TestResult.WorkTest.ChangeFre((float)(OrFre + fre));
                OrFre += fre;

            }
            catch
            {
                ShowHide("增加频率失败" + "\r\n" + "请检查串口和电源");
            }
        }

        public void SendPra(byte channel, byte extennum, byte range, float amp, float tannum)
        {
            TestResult.WorkTest.CorrectionPra(channel, extennum, range, amp, tannum);
        }
        public double OrFre { get; set; } = 50.0;
        public void MulFre(double fre)
        {
            if (OrFre > fre)
            {
                try
                {
                    TestResult.WorkTest.ChangeFre((float)(OrFre - fre));
                    OrFre -= fre;
                }
                catch
                {
                    ShowHide("降低频率失败" + "\r\n" + "请检查串口和电源");
                }
            }
        }

        public Thread T1 = new Thread(BoomFive);

        public static void BoomFive()
        {
            while (true)
            {
                Thread.Sleep(5000);
                TestResult.WorkTest.BoomFive();
            }
        }


        #endregion
        #region chart
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public List<string> Labels2 { get; set; }
        public SeriesCollection SeriesCollection3 { get; set; }
        public List<string> Labels3 { get; set; }
        public SeriesCollection SeriesCollection4 { get; set; }
        public List<string> Labels4 { get; set; }
        public SeriesCollection SeriesCollectionEleYAndVolate { get; set; }
        public SeriesCollection SeriesCollectionEleYAndVolate1 { get; set; }

        #region 介损电晕耐压图
        public SeriesCollection TanEleVolate { get; set; }
        public SeriesCollection panel1SeriesCollection { get; set; }
        public SeriesCollection panel2SeriesCollection { get; set; }
        public SeriesCollection panel3SeriesCollection { get; set; }
        public SeriesCollection panel4SeriesCollection { get; set; }

        public ChartValues<ObservablePoint> TanEleVolatevalue { get; set; }
        public ChartValues<ObservablePoint> panel1ObservablePoint
        {
            get;
            set;
        }
        public ChartValues<ObservablePoint> panel2ObservablePoint { get; set; }
        public ChartValues<ObservablePoint> panel3ObservablePoint { get; set; }
        public ChartValues<ObservablePoint> panel4ObservablePoint { get; set; }
        public ObservableCollection<TestResultDataGrid> DatagridData { get; set; }
        public void CreateTanEleVolate()
        {
            TanEleVolatevalue = new ChartValues<ObservablePoint>();
            LineSeries t1 = new LineSeries
            {
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = TanEleVolatevalue
            };
            TanEleVolate = new SeriesCollection { };
            TanEleVolate.Add(t1);
            TanEleVolate[0].Values.Add(new ObservablePoint(0, 0));

            XAllFormat = val => ((int)val).ToString() + "s";
            YAllFormat = val => (val / 1000.0).ToString("N3") + " kV";
        }

        public void CreateFourTan()
        {
            panel1ObservablePoint = new ChartValues<ObservablePoint>();
            panel2ObservablePoint = new ChartValues<ObservablePoint>();
            panel3ObservablePoint = new ChartValues<ObservablePoint>();
            panel4ObservablePoint = new ChartValues<ObservablePoint>();
            LineSeries t1 = new LineSeries
            {
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = panel1ObservablePoint
            };
            LineSeries t2 = new LineSeries
            {
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = panel2ObservablePoint
            };
            LineSeries t3 = new LineSeries
            {
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = panel3ObservablePoint
            };
            LineSeries t4 = new LineSeries
            {
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = panel4ObservablePoint
            };

            panel1SeriesCollection = new SeriesCollection { };
            panel1SeriesCollection.Add(t1);
            panel2SeriesCollection = new SeriesCollection { };
            panel2SeriesCollection.Add(t2);
            panel3SeriesCollection = new SeriesCollection { };
            panel3SeriesCollection.Add(t3);
            panel4SeriesCollection = new SeriesCollection { };
            panel4SeriesCollection.Add(t4);
            panel1SeriesCollection[0].Values.Add(new ObservablePoint(0, 0));
            panel2SeriesCollection[0].Values.Add(new ObservablePoint(0, 0));
            panel3SeriesCollection[0].Values.Add(new ObservablePoint(0, 0));
            panel4SeriesCollection[0].Values.Add(new ObservablePoint(0, 0));

            YFormatter = val => (val * 100.0).ToString("N3") + "%";
            XFormatter = val => (val / 1000.0).ToString("N3") + " kV";
        }

        public void AddFourPanelData(ChartPannel chartPannel, double Xva, double yVa)
        {
            ObservablePoint a = new ObservablePoint(Xva, yVa);
            if (chartPannel == ChartPannel.Channel1)
                panel1SeriesCollection[0].Values.Add(a);
            if (chartPannel == ChartPannel.Channel2)
                panel2SeriesCollection[0].Values.Add(a);
            if (chartPannel == ChartPannel.Channel3)
                panel3SeriesCollection[0].Values.Add(a);
            if (chartPannel == ChartPannel.Channel4)
                panel4SeriesCollection[0].Values.Add(a);
        }
        public void AddTanEleVolatepoint(double Xva, double yVa)
        {
            ObservablePoint a = new ObservablePoint(Xva, yVa);
            TanEleVolate[0].Values.Add(a);
        }



        #endregion
        public ObservablePoint[] getobs(double[] chartdata, List<string> Xvalue)
        {
            if (chartdata.Length == Xvalue.Count)
            {
                float[] xva = new float[Xvalue.Count];
                string[] tmp = Xvalue.ToArray();
                for (int i = 0; i < Xvalue.Count; i++)
                {
                    xva[i] = (float)NumericsConverter.Text2Value(tmp[i]).value;
                }
                ObservablePoint[] obs = new ObservablePoint[Xvalue.Count];
                for (int i = 0; i < Xvalue.Count; i++)
                {
                    obs[i] = new ObservablePoint(xva[i], chartdata[i]);
                }
                return obs;
            }

            return null;
        }


        public void SetChartObserver(double[] chartdata, List<string> Xvalue, ChartPannel pannel)
        {
            ChartValues<ObservablePoint> c1 = new ChartValues<ObservablePoint>();
            c1.AddRange(getobs(chartdata, Xvalue));
            LineSeries t1 = new LineSeries
            {
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = c1,
            };
            if (pannel == ChartPannel.Channel1)
            {
                SeriesCollection = new SeriesCollection { };
                SeriesCollection.Add(t1);
            }
            if (pannel == ChartPannel.Channel2)
            {
                SeriesCollection2 = new SeriesCollection { };
                SeriesCollection2.Add(t1);
            }
            if (pannel == ChartPannel.Channel3)
            {
                SeriesCollection3 = new SeriesCollection { };
                SeriesCollection3.Add(t1);
            }
            if (pannel == ChartPannel.Channel4)
            {
                SeriesCollection4 = new SeriesCollection { };
                SeriesCollection4.Add(t1);
            }

            //XFormatter = val => (int)val;
            //YFormatter = val => val.ToString("N") + " V";
        }


        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public Func<double, string> XAllFormat { get; set; }
        public Func<double, string> YAllFormat { get; set; }
        #endregion
        public MainWindowModel()
        {
            // Task.Factory.StartNew(OpenNewTestWindow);
        }
    }

    public enum ChartPannel
    {
        Channel1 = 0,
        Channel2,
        Channel3,
        Channel4
    }

    public delegate void AllAutoTestFlag(byte[] ISopen);

    public class TestResultDataGrid
    {
        public string volate { get; set; }
        public string captance1 { get; set; }
        public string tan1 { get; set; }
        public string captance2 { get; set; }
        public string tan2 { get; set; }
        public string captance3 { get; set; }
        public string tan3 { get; set; }
        public string captance4 { get; set; }
        public string tan4 { get; set; }
        public string Qultitly { get; set; }

    }
}
