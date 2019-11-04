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

namespace HV9003TE4
{

    public class AllAutoTestModel : INotifyPropertyChanged
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

        public ObservableCollection<string> projiectnames { get; set; } = new ObservableCollection<string>();//方案列表

        public ObservableCollection<string> ProjectVolate { get; set; } = new ObservableCollection<string>();//添加的方案数据
        public List<int> TimeSources { get; set; } = new List<int>();
        public bool IsCompleteVolateTest { get; set; } = true;
        public List<double> Yvalue1 { get; set; } = new List<double>();
        public List<double> Yvalue2 { get; set; } = new List<double>();
        public List<float> Xvalue1 { get; set; } = new List<float>();
        public List<float> Xvalue2 { get; set; } = new List<float>();
        public Visibility QuqlityIsOk { get; set; } = Visibility.Collapsed;

        public ObservableCollection<UIElement> Projectdataui { get; set; }
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

        }

        private void TcpServer_ClientConnected(object sender, AsyncEventArgs e)

        {

        }
        public bool IsTcpTestting { get; set; } = false;

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
            if (Temp[0] == 0xdd && Temp[1] == 0x0a)
            {
                byte[] array1 = System.Text.Encoding.ASCII.GetBytes("OK");
                TestClass.QueryTestResult(TcpTask.TcpServer, null, array1);
               // OpenAutoTest(Temp);
                //IsTcpTestting = true;
                SysData = Temp;
                try
                {
                    if (Temp[2] == 0x01) AllTestResult.PanelEnable = true; else AllTestResult.PanelEnable = false;
                    if (Temp[3] == 0x01) AllTestResult.Pane2Enable = true; else AllTestResult.Pane2Enable = false;
                    if (Temp[4] == 0x01) AllTestResult.Pane3Enable = true; else AllTestResult.Pane3Enable = false;
                    if (Temp[5] == 0x01) AllTestResult.Pane4Enable = true; else AllTestResult.Pane4Enable = false;
                }
                catch { }
                //FileListItems(Temp);
                //StartAutobytcp();

            }
            if (Temp[0] == 0xdd && Temp[1] == 0x0b)
            {
                if (IsTcpTestting)
                {
                    byte[] array = System.Text.Encoding.ASCII.GetBytes("BUSY");
                    TestClass.QueryTestResult(TcpTask.TcpServer, null, array);
                }
                else
                {
                    TestClass.QueryTestResult(TcpTask.TcpServer, null, StaticClass.Getbytesdata(AllTestResult, 2));
                }
            }
            Models.StaticClass.GetDataForTcpAutoTest(Temp);
            TestMesseagesNull.ReturnMessages(TcpTask.TcpServer, Temp);
        }


        private void FileListItems(byte[] data)
        {
            //ThreadPool.QueueUserWorkItem(delegate
            //{
            //    SynchronizationContext.SetSynchronizationContext(new
            //    System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
            //    SynchronizationContext.Current.Post(pl =>
            //    {
            ListboxItemsources.Clear();
            // ListboxItemsources.Add("远程数据显示\t\n若需要手动测量\t\n请设置电晕和耐压");
            var a = StaticClass.GetDataForTcpAutoTest(data);
            foreach (var b in a.NeedTestList)
            {
                PhysicalVariable tpd = NumericsConverter.Text2Value(b.ToString() + "V");
                ListboxItemsources.Add("待测电压  :" + tpd);
            }
            PhysicalVariable eley = NumericsConverter.Text2Value(a.EleY.ToString() + "V");
            PhysicalVariable elevolate = NumericsConverter.Text2Value(a.EleVolate.ToString() + "V");
            ListboxItemsources.Add("待测电晕  :" + eley);
            ListboxItemsources.Add("持续时间  :" + a.HideTime.ToString() + ":" + "  耐    压:" + elevolate);
            //    }, null);
            //});
        }
        private void SetVolate(float volate)
        {
            SetBaseVolate(volate);
            Thread.Sleep(300);
            UpVolate();
        }
        #region AUTO
        public bool ISELEY { get; set; } = false;
        public bool ISELEVOLATE { get; set; } = false;
        public bool TestByProject { get; set; } = false;

        public FourTestResult AllTestResult { get; set; } = new FourTestResult();
        public void StartAutoTest(Models.SysAutoTestResult sys)
        {
            if (IsTcpTestting)
                sys = StaticClass.GetDataForTcpAutoTest(SysData);
            #region 电压
            Models.AutoStateStatic.SState.Clear();
            float[] needtest = sys.NeedTestList.ToArray();
            AllTestResult.NeedTestNum = needtest.Length;
            for (int i = 0; i < sys.NeedTestList.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                resetEvent.WaitOne();
                // SetVolate(needtest[i]);
                //IsEnable = false;
                int p = 0;
                bool IsEnd = false;
                Models.AutoStateStatic.SState.TestText.Add("电压  ：" + needtest[i].ToString() + "V" + ":\t正在升压中...");
                STAMethod();
                while (!IsEnable)
                {
                    p++;
                    Thread.Sleep(100);
                    if (p > 200)
                    {
                        Models.AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                        Models.AutoStateStatic.SState.Process = i + 1;
                        Models.AutoStateStatic.SState.TaskCount = needtest.Length;
                        Models.AutoStateStatic.SState.TestText.Add("电压  ：" + needtest[i].ToString() + "V" + ":\t升压超时");
                        STAMethod();
                        StaticClass.ReturntestTestData(AllTestResult, Capacitance1, Capacitance2, Capacitance3, Capacitance4, DissipationFactor1, DissipationFactor2,
                            DissipationFactor3, DissipationFactor4, false);
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            SynchronizationContext.SetSynchronizationContext(new
                            System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                            SynchronizationContext.Current.Post(pl =>
                              {
                                  float Processq = (float)AutoStateStatic.SState.Process / (float)AutoStateStatic.SState.TaskCount;
                                  Process = Processq.ToString("00%");
                                  ProcesNum = (int)(Processq * 100);

                              }, null);
                        });

                        IsEnd = true;
                        break;
                    }
                }
                if (!IsEnd)
                {
                    Models.AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                    Models.AutoStateStatic.SState.Process = i + 1;
                    Models.AutoStateStatic.SState.TaskCount = needtest.Length;
                    Models.AutoStateStatic.SState.TestText.Add("电压  ：" + needtest[i].ToString() + "V" + ":\t升压完成" + "" +
                        "\t\n通道一电容：" + Capacitance1 + "\t\t介损 ：" + DissipationFactor1 +
                        "\t\n通道二电容：" + Capacitance2 + "\t\t介损 ：" + DissipationFactor2 +
                        "\t\n通道三电容：" + Capacitance3 + "\t\t介损 ：" + DissipationFactor3 +
                        "\t\n通道四电容：" + Capacitance4 + "\t\t介损 ：" + DissipationFactor4

                        );
                    StaticClass.ReturntestTestData(AllTestResult, Capacitance1, Capacitance2, Capacitance3, Capacitance4, DissipationFactor1, DissipationFactor2,
                          DissipationFactor3, DissipationFactor4, true);
                    STAMethod();
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        SynchronizationContext.SetSynchronizationContext(new
                        System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                        SynchronizationContext.Current.Post(pl =>
                        {
                            float Processq = (float)AutoStateStatic.SState.Process / (float)AutoStateStatic.SState.TaskCount;
                            Process = Processq.ToString("00%");
                            ProcesNum = (int)(Processq * 100);

                        }, null);
                    });
                }
            }
            #endregion
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(async pl =>
                {
                    if (sys.IsEleY)
                    {
                        StaticClass.ShowELEYANDVOLATe(Views.EleOrVolate.EleY);
                        if (AutoStateStatic.SState.IsStartEleY)
                            await Task.Factory.StartNew(StartEleY);
                        else
                        {
                            if (sys.IsVolate)
                            {
                                StaticClass.ShowELEYANDVOLATe(Views.EleOrVolate.Volate);
                                if (AutoStateStatic.SState.IsStartVolate)
                                {
                                    await Task.Factory.StartNew(StartVolate);
                                }
                            }
                        }
                    }
                }, null);
            });
        }
        private void StartTestTask()
        {
            StartAutoTest(GetSys());
        }

        public byte[] SysData { get; set; }
        private void StartTestTaskByTcp()
        {
            StartAutoTest(StaticClass.GetDataForTcpAutoTest(SysData));
        }
        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        static CancellationToken token = tokenSource.Token;
        static ManualResetEvent resetEvent = new ManualResetEvent(true);
        public Task task { get; set; }
        public Task taskTcp { get; set; }
        public Task taskProject { get; set; }
        public SysAutoTestResult SysProject { get; set; }

        public void StartAuto()
        {
            //Task.Factory.StartNew(StartTestTask);
            task = new Task(StartTestTask, token);
            task.Start();
        }
        public void StartAutobytcp()
        {
            //Task.Factory.StartNew(StartTestTask);
            taskTcp = new Task(StartTestTaskByTcp, token);
            taskTcp.Start();
        }

        public void StartAutobyProject()
        {
            //Task.Factory.StartNew(StartTestTask);
            taskProject = new Task(StartTestTaskByProject, token);
            taskProject.Start();
        }
        private void StartTestTaskByProject()
        {
            StartAutoTest(SysProject);
        }
        public void ResetTest()
        {
            resetEvent.Reset();
        }
        public void ContinuTest()
        {
            resetEvent.Set();
        }
        public volatile bool IsStartEleY = false;
        public bool NoRigthtEnd { get; set; } = false;
        public bool IsCompleteEleTest { get; set; } = true;

        private void CreateWave(bool ISELEORVOLATE, int p = 0)
        {
            if (ISELEORVOLATE)
            {
                Yvalue1.Add((double)HVVoltage.value);
                Xvalue1.Add(p);
                SetEleYAndVolate(Xvalue1, Yvalue1, true);
            }
            else
            {
                Yvalue1.Add((double)HVVoltage.value);
                Xvalue1.Add(p);
                SetEleYAndVolate(Xvalue1, Yvalue1, false);
            }

        }
        private void RecorEley()
        {
            int p = 0;
            while (true)
            {
                CreateWave(true, p);
                p++;
                Thread.Sleep(1000);
            }
        }

        public void StartEleY()
        {
            Models.SysAutoTestResult sys = new SysAutoTestResult();
            if (IsTcpTestting)
                sys = StaticClass.GetDataForTcpAutoTest(SysData);
            else
            {
                sys = GetSys();
            }
            if (IsCompleteVolateTest)
            {
                //  SetVolate(sys.EleY);
                int p = 0;
                bool IsEnd = false;
                Models.AutoStateStatic.SState.TestText.Add("电晕  ：" + sys.EleY.ToString() + "V" + ":\t正在升压中...");
                STAMethod();
                while (!IsEnable)
                {
                    p++;
                    Thread.Sleep(1000);

                    if (p > 20)
                    {
                        Models.AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                        Models.AutoStateStatic.SState.TestText.Add("电晕  ：" + sys.EleY.ToString() + "V" + ":\t升压超时");
                        StaticClass.AddEleY(AllTestResult, HVVoltage,
                         HVVoltage, HVVoltage, HVVoltage);
                        STAMethod();
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            SynchronizationContext.SetSynchronizationContext(new
                            System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                            SynchronizationContext.Current.Post(async pl =>
                            {
                                if (sys.IsVolate)
                                {
                                    StaticClass.ShowELEYANDVOLATe(Views.EleOrVolate.Volate);
                                    if (AutoStateStatic.SState.IsStartVolate == true)
                                        await Task.Factory.StartNew(StartVolate);
                                    else
                                        AutoStateStatic.SState.IsStartVolate = false;
                                }
                                else
                                {
                                    IsTcpTestting = false;
                                }
                            }, null);
                        });
                        IsEnd = true;
                        break;
                    }
                }
                if (!IsEnd)
                {
                    AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                    AutoStateStatic.SState.TestText.Add("电晕  ：" + sys.EleY.ToString() + "V" + ":\t升压完成");
                    StaticClass.AddEleY(AllTestResult, HVVoltage,
                         HVVoltage, HVVoltage, HVVoltage);
                    STAMethod();
                    Task.Factory.StartNew(RecorEley);
                    QuqlityIsOk = Visibility.Visible;
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        SynchronizationContext.SetSynchronizationContext(new
                        System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                        SynchronizationContext.Current.Post(async pl =>
                        {
                            //Views.Qualified q = new Views.Qualified
                            //{
                            //    WindowStartupLocation = WindowStartupLocation.CenterScreen
                            //};
                            //q.ShowDialog();
                            StaticClass.ShowELEYANDVOLATe(Views.EleOrVolate.Volate);
                            if (AutoStateStatic.SState.IsStartVolate == true)
                                await Task.Factory.StartNew(StartVolate);
                            else
                                AutoStateStatic.SState.IsStartVolate = false;
                            Models.AutoStateStatic.SState.IsPress = false;

                        }, null);
                    });
                }
            }

        }
        public void StartVolate()
        {
            Models.SysAutoTestResult sys = new SysAutoTestResult();
            if (IsTcpTestting)
                sys = StaticClass.GetDataForTcpAutoTest(SysData);
            else
            {
                sys = GetSys();
            }
            if (IsCompleteEleTest)
            {
                //  SetVolate(sys.EleVolate);
                int p = 0;
                bool IsEnd = false;
                Models.AutoStateStatic.SState.TestText.Add("耐压  ：" + sys.EleVolate.ToString() + "V" + ":\t正在升压中...");
                STAMethod();
                while (!IsEnable)
                {
                    p++;
                    Thread.Sleep(1000);
                    CreateWave(false, p);
                    float maxvalue = 0;
                    float actvalue = 0;
                    if (p > 20)
                    {
                        if (Math.Abs((float)HVVoltage.value - sys.EleVolate) < maxvalue)
                            actvalue = (float)HVVoltage.value;
                        AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                        AutoStateStatic.SState.TestText.Add("耐压  ：" + actvalue.ToString() + "V" + ":\t未升到耐压值");
                        StaticClass.AddEleVolate(AllTestResult, HVVoltage, HVVoltage, HVVoltage, HVVoltage
                 , sys.HideTime, sys.HideTime, sys.HideTime, sys.HideTime);
                        STAMethod();
                        IsEnd = true;
                        IsTcpTestting = false;
                        break;
                    }
                }
                if (!IsEnd)
                {
                    AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                    AutoStateStatic.SState.TestText.Add("耐压  ：" + sys.EleVolate.ToString() + "V" + ":\t升压完成");
                    STAMethod();
                    int c = 0;
                    float MaxVolate = 0;
                    Models.AutoStateStatic.SState.TestText.Add("当前电压： " + HVVoltage + "\t\n正在持续耐压中...");
                    STAMethod();
                    #region 
                    for (int i = 0; i < sys.HideTime; i++)
                    {

                        Models.AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                        Models.AutoStateStatic.SState.TestText.Add("当前电压： " + HVVoltage + "\t\n正在持续耐压中...\t\t" + (sys.HideTime - i - 1).ToString() + "S");
                        STAMethod();
                        CreateWave(false, p + i + 1);
                        if (StaticClass.IsOk((float)HVVoltage.value, sys.EleVolate))
                        {
                            float tempdata = Math.Abs(sys.EleVolate - (float)HVVoltage.value);
                            if (tempdata > MaxVolate)
                            {
                                MaxVolate = tempdata;
                                AutoStateStatic.SState.MaxEqualVolate = sys.EleVolate;
                            }
                            Thread.Sleep(1000);
                            c++;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            c++;
                            continue;
                        }
                    }
                    if (((float)AutoStateStatic.SState.NoSame / (float)AutoStateStatic.SState.AllNum) >= 0.80f)
                    {
                        Models.AutoStateStatic.SState.TestText.Add("耐压实验已完成  耐压结果： 不合格");
                        StaticClass.AddEleVolate(AllTestResult, HVVoltage, HVVoltage, HVVoltage, HVVoltage
                  , sys.HideTime, sys.HideTime, sys.HideTime, sys.HideTime);
                        STAMethod();
                        AutoStateStatic.SState.NaiVolate = false;//耐压是否合格
                        AutoStateStatic.SState.CompeleteVolate = true;//是否完成耐压
                        IsTcpTestting = false;
                    }
                    else
                    {
                        Models.AutoStateStatic.SState.TestText.Add("耐压实验已完成  耐压结果： 合格");
                        STAMethod();
                        StaticClass.AddEleVolate(AllTestResult, HVVoltage, HVVoltage, HVVoltage, HVVoltage
               , sys.HideTime, sys.HideTime, sys.HideTime, sys.HideTime);
                        AutoStateStatic.SState.NaiVolate = true;//耐压是否合格
                        AutoStateStatic.SState.CompeleteVolate = true;//是否完成耐压
                        IsTcpTestting = false;
                    }
                    #endregion
                }
            }
        }
        public void STAMethod()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(pl =>
                {
                    TestResultMeassge.Clear();
                    foreach (var a in AutoStateStatic.SState.TestText)
                    {
                        TestResultMeassge.Add(a);
                    }

                }, null);
            });
        }
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
            double tan = Math.Tan(pnv(AGx.value) - pnv(AGn.value));
            if (Math.Abs(tan) < 0.0000005)
                return "0.000%";
            if ((tan < 1e24) && (tan > -1e24))
                return NumericsConverter.Value2Text(tan, 4, -5, "", SCEEC.Numerics.Quantities.QuantityName.None, percentage: true).Trim();
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
        public event ReturnResult OutTestResult;
        private void WorkTest_OutTestResult(byte[] result)
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
            OutTestResult(result);


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
                TestClass.QueryTestResult(TcpTask.TcpServer, null, new byte[3] { 0xdd, 0x01, 0x01 });
            }
            catch
            {
                VolateState = false;
                TestClass.QueryTestResult(TcpTask.TcpServer, null, new byte[3] { 0xdd, 0x01, 0x02 });
                ShowHide("开启电源失败" + "\r\n" + "请检查串口和电源");
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
                TestResult.WorkTest.startUpVolate();
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
        public void SetBaseVolate(float vol)
        {
            try
            {
                TestResult.WorkTest.ChangeVolate(vol);
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

        /// <summary>
        /// 定义样式
        /// </summary>
        /// <param name="chartdata"></param>
        public void SetChart1(double[] chartdata, List<string> Xvalue)
        {
            LineSeries t1 = new LineSeries
            {
                Values = new ChartValues<double>(chartdata),
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49))


            };
            Labels = Xvalue;
            SeriesCollection = new SeriesCollection { };
            SeriesCollection.Add(t1);

        }


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

        public void SetEleYAndVolate(List<float> EleXvalue, List<double> EleYvalue, bool OneOrTwo)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(async pl =>
                {
                    if (OneOrTwo)
                    {
                        ChartValues<ObservablePoint> c1 = new ChartValues<ObservablePoint>();
                        c1.AddRange(StaticClass.GetEleOrVolate(EleXvalue, EleYvalue));
                        LineSeries t1 = new LineSeries
                        {
                            StrokeThickness = 2,
                            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                            Fill = System.Windows.Media.Brushes.Transparent,
                            LineSmoothness = 0,//0为折现样式
                            PointGeometrySize = 0,//无点样式
                            PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                            Values = c1,
                        };
                        SeriesCollectionEleYAndVolate = new SeriesCollection();
                        SeriesCollectionEleYAndVolate.Add(t1);
                    }
                    else
                    {

                        ChartValues<ObservablePoint> c2 = new ChartValues<ObservablePoint>();
                        c2.AddRange(StaticClass.GetEleOrVolate(EleXvalue, EleYvalue));

                        LineSeries t2 = new LineSeries
                        {
                            StrokeThickness = 2,
                            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                            Fill = System.Windows.Media.Brushes.Transparent,
                            LineSmoothness = 0,//0为折现样式
                            PointGeometrySize = 8,
                            PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                            Values = c2,
                        };
                        SeriesCollectionEleYAndVolate1 = new SeriesCollection();
                        SeriesCollectionEleYAndVolate1.Add(t2);
                    }
                }, null);
            });
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
                //c1.Clear();
            }

            //XFormatter = val => (int)val;
            //YFormatter = val => val.ToString("N") + " kV";
        }
        public Func<double, int> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        #endregion

    }



}
