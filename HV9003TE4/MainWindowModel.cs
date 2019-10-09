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

namespace HV9003TE4
{
    public delegate void ReturnResult(byte[] data);

    public class MainWindowModel : INotifyPropertyChanged
    {
        TestMesseages TestMesseagesNull = new TestMesseages();
        public volatile byte[] TestRes;

        public bool IsEnable { get; set; } = false;
        public string YPopu { get; set; }
        public bool ISREMOTE { get; set; } = true;
        public bool VolateState { get; set; } = false;
        public bool ISZERO { get; set; } = true;
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
            TestMesseagesNull.ReturnMessages(TcpTask.TcpServer, Temp);
        }

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
                    }
                    if (temp[2] == 0x02)
                    {
                        ISREMOTE = true;
                        IsEnable = true;
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
                IsEnable = false;
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
                TestClass.QueryTestResult(TcpTask.TcpServer, null,new byte[3] { 0xdd,0x01,0x01});
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
                TestClass.SendData(new byte[3] { 0xdd,0x04,0x01}, TcpTask.TcpServer);
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

        private double _trend;
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
                //c1.Clear();
            }
            if (pannel == ChartPannel.Channel2)
            {
                SeriesCollection2 = new SeriesCollection { };
                SeriesCollection2.Add(t1);
                //c1.Clear();
            }
            if (pannel == ChartPannel.Channel3)
            {
                SeriesCollection3 = new SeriesCollection { };
                SeriesCollection3.Add(t1);
                //c1.Clear();
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
        public MainWindowModel()
        {

        }
    }

    public enum ChartPannel
    {
        Channel1 = 0,
        Channel2,
        Channel3,
        Channel4
    }
}
