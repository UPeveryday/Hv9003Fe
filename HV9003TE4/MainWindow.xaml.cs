using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using SCEEC.Numerics;
using System.Timers;
using HV9003TE4.Models;
using System.Collections.ObjectModel;

namespace HV9003TE4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        MainWindowModel mv;
        public MainWindow()
        {
            InitializeComponent();
            mv = new MainWindowModel();
            this.DataContext = mv;
            mv.StartTcp();
            try
            {
                mv.StartRecCom();
                mv.SetFre(50);
                mv.T1.IsBackground = true;
                mv.T1.Start();
                mv.OutTestResult += MainWindow_OutTestResult;
                InitChart();
            }
            catch
            {
                mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
                //this.Close();
            }
            mv.OpenAutoTest += MainWindow_OpenAutoTest;
        }
        private void MainWindow_OpenAutoTest(byte[] ISopen)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(pl =>
                {
                    Views.AllAutoTest al = new Views.AllAutoTest(ISopen);
                    al.Show();

                }, null);
            });

        }

        private void MainWindow_OutTestResult(byte[] data)
        {

            if (data[58] == 1)
            {
                Views.Alarm a1 = new Views.Alarm("电压或频率设置失败");
                a1.ShowDialog();
            }
            if (data[58] == 2)
            {
                Views.Alarm a1 = new Views.Alarm("读取电流或者功率失败");
                a1.ShowDialog();
            }
            if (data[58] == 3)
            {
                Views.Alarm a1 = new Views.Alarm("变频电源过流");
                a1.ShowDialog();
            }
            if (data[58] == 4)
            {
                Views.Alarm a1 = new Views.Alarm("变频电源开启或者输出失败");
                a1.ShowDialog();
            }
            if (data[58] == 6)
            {
                Views.Alarm a1 = new Views.Alarm("被实测无信号");
                a1.ShowDialog();
            }
            if (data[58] == 7)
            {
                Views.Alarm a1 = new Views.Alarm("测量版档位过流");
                a1.ShowDialog();
            }
            if (data[58] == 8)
            {
                Views.Alarm a1 = new Views.Alarm("心跳丢失");
                a1.ShowDialog();
            }
            // throw new NotImplementedException();

        }

        public float NeedVolate { get; set; } = 0;



        private void Mul_01__Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.MulFre(0.1);
        }

        private void Mul_1_Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.MulFre(1);

        }

        private void Mul_10_Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.MulFre(10);
        }

        private void Mul_50_Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.MulFre(50);
        }

        private void Add_01__Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.AddFre(0.1);
        }

        private void Add_1_Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.AddFre(1);
        }

        private void Add_10_Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.AddFre(10);
        }

        private void Add_50_Fre_Click(object sender, RoutedEventArgs e)
        {
            mv.AddFre(50);
        }
        public List<string> XVolate { get; set; } = new List<string>();
        public List<double> YVolateAndCn { get; set; } = new List<double>();
        public List<double> YVolateAndCn2 { get; set; } = new List<double>();
        public List<double> YVolateAndCn3 { get; set; } = new List<double>();
        public List<double> YVolateAndCn4 { get; set; } = new List<double>();
        public List<string> XFre { get; set; } = new List<string>();

        public List<double> YVolateAndCurrent { get; set; } = new List<double>();
        public List<double> YVolateAndCurrent2 { get; set; } = new List<double>();
        public List<double> YVolateAndCurrent3 { get; set; } = new List<double>();
        public List<double> YVolateAndCurrent4 { get; set; } = new List<double>();
        public List<string> XTime { get; set; } = new List<string>();

        public List<double> YVolateAndTan { get; set; } = new List<double>();
        public List<double> YVolateAndTan2 { get; set; } = new List<double>();
        public List<double> YVolateAndTan3 { get; set; } = new List<double>();
        public List<double> YVolateAndTan4 { get; set; } = new List<double>();



        private void InitChart()
        {
            XVolate.Add("0");
            YVolateAndCn.Add(0);
            YVolateAndCn2.Add(0);
            YVolateAndCn3.Add(0);
            YVolateAndCn4.Add(0);
            XFre.Add("0");
            YVolateAndCurrent.Add(0);
            YVolateAndCurrent2.Add(0);
            YVolateAndCurrent3.Add(0);
            YVolateAndCurrent4.Add(0);
            XTime.Add("0");
            YVolateAndTan.Add(0);
            YVolateAndTan2.Add(0);
            YVolateAndTan3.Add(0);
            YVolateAndTan4.Add(0);
        }
        private void Recor_Click(object sender, RoutedEventArgs e)
        {

            #region 设置基础值
            double VolateD = 0;
            string Volate = "0V";
            try
            {
                if (mv.HVVoltage.ToString() != "NaN")
                {
                    Volate = mv.HVVoltage.ToString();
                    PhysicalVariable a = NumericsConverter.Text2Value(Volate);
                    VolateD = (double)a.value;
                }
            }
            catch
            {

            }
            var Fre = mv.HVFrequency.ToString();
            var Time = DateTime.Now.ToString();

            float Cn = (float)NumericsConverter.Text2Value(mv.Capacitance1).value;
            var Current = (float)NumericsConverter.Text2Value(mv.Current1).value;
            var Tan = (float)NumericsConverter.Text2Value(mv.DissipationFactor1).value;

            var Cn2 = (float)NumericsConverter.Text2Value(mv.Capacitance2).value;
            var Current2 = (float)NumericsConverter.Text2Value(mv.Current2).value;
            var Tan2 = (float)NumericsConverter.Text2Value(mv.DissipationFactor2).value;

            var Cn3 = (float)NumericsConverter.Text2Value(mv.Capacitance3).value;
            var Current3 = (float)NumericsConverter.Text2Value(mv.Current3).value;
            var Tan3 = (float)NumericsConverter.Text2Value(mv.DissipationFactor3).value;

            var Cn4 = (float)NumericsConverter.Text2Value(mv.Capacitance4).value;
            var Current4 = (float)NumericsConverter.Text2Value(mv.Current4).value;
            var Tan4 = (float)NumericsConverter.Text2Value(mv.DissipationFactor4).value;
            if (Com1.SelectedIndex == 0)
            {
                XVolate.Add(Volate);

                if (Com2.SelectedIndex == 0)
                {
                    YVolateAndCn.Add(Cn);
                    YVolateAndCn2.Add(Cn2);
                    YVolateAndCn3.Add(Cn3);
                    YVolateAndCn4.Add(Cn4);

                    mv.SetChartObserver(YVolateAndCn.ToArray(), XVolate, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCn2.ToArray(), XVolate, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCn3.ToArray(), XVolate, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCn4.ToArray(), XVolate, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 1)
                {
                    YVolateAndCurrent.Add((double)Current);
                    YVolateAndCurrent2.Add((double)Current2);
                    YVolateAndCurrent3.Add((double)Current3);
                    YVolateAndCurrent4.Add((double)Current4);
                    mv.SetChartObserver(YVolateAndCurrent.ToArray(), XVolate, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCurrent2.ToArray(), XVolate, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCurrent3.ToArray(), XVolate, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCurrent4.ToArray(), XVolate, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 2)
                {
                    YVolateAndTan.Add((double)Tan);
                    YVolateAndTan2.Add((double)Tan2);
                    YVolateAndTan3.Add((double)Tan3);
                    YVolateAndTan4.Add((double)Tan4);
                    mv.SetChartObserver(YVolateAndTan.ToArray(), XVolate, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndTan2.ToArray(), XVolate, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndTan3.ToArray(), XVolate, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndTan4.ToArray(), XVolate, ChartPannel.Channel4);

                }

            }
            if (Com1.SelectedIndex == 1)
            {
                XFre.Add(Fre);
                if (Com2.SelectedIndex == 0)
                {
                    YVolateAndCn.Add((double)Cn);
                    YVolateAndCn2.Add((double)Cn2);
                    YVolateAndCn3.Add((double)Cn3);
                    YVolateAndCn4.Add((double)Cn4);
                    mv.SetChartObserver(YVolateAndCn.ToArray(), XFre, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCn2.ToArray(), XFre, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCn3.ToArray(), XFre, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCn4.ToArray(), XFre, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 1)
                {
                    YVolateAndCurrent.Add((double)Current);
                    YVolateAndCurrent2.Add((double)Current2);
                    YVolateAndCurrent3.Add((double)Current3);
                    YVolateAndCurrent4.Add((double)Current4);
                    mv.SetChartObserver(YVolateAndCurrent.ToArray(), XFre, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCurrent2.ToArray(), XFre, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCurrent3.ToArray(), XFre, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCurrent4.ToArray(), XFre, ChartPannel.Channel4);

                }
                else
                {
                    YVolateAndTan.Add((double)Tan);
                    YVolateAndTan2.Add((double)Tan2);
                    YVolateAndTan3.Add((double)Tan3);
                    YVolateAndTan4.Add((double)Tan4);
                    mv.SetChartObserver(YVolateAndTan.ToArray(), XFre, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndTan2.ToArray(), XFre, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndTan3.ToArray(), XFre, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndTan4.ToArray(), XFre, ChartPannel.Channel4);

                }
            }
            if (Com1.SelectedIndex == 2)
            {
                XFre.Add(Time);
                if (Com2.SelectedIndex == 0)
                {
                    YVolateAndCn.Add((double)VolateD);
                    YVolateAndCn2.Add((double)VolateD);
                    YVolateAndCn3.Add((double)VolateD);
                    YVolateAndCn4.Add((double)VolateD);
                    mv.SetChartObserver(YVolateAndCn.ToArray(), XTime, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCn2.ToArray(), XTime, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCn3.ToArray(), XTime, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCn4.ToArray(), XTime, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 1)
                {
                    YVolateAndCurrent.Add((double)Current);
                    YVolateAndCurrent2.Add((double)Current2);
                    YVolateAndCurrent3.Add((double)Current3);
                    YVolateAndCurrent4.Add((double)Current4);
                    mv.SetChartObserver(YVolateAndCurrent.ToArray(), XTime, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCurrent2.ToArray(), XTime, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCurrent3.ToArray(), XTime, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCurrent4.ToArray(), XTime, ChartPannel.Channel4);

                }
                else
                {
                    YVolateAndTan.Add((double)Tan);
                    YVolateAndTan2.Add((double)Tan2);
                    YVolateAndTan3.Add((double)Tan3);
                    YVolateAndTan4.Add((double)Tan4);
                    mv.SetChartObserver(YVolateAndTan.ToArray(), XTime, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndTan2.ToArray(), XTime, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndTan3.ToArray(), XTime, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndTan4.ToArray(), XTime, ChartPannel.Channel4);

                }
            }
            #endregion
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (XVolate.Count > 1) XVolate.RemoveAt(XVolate.Count - 1);
            if (YVolateAndCn.Count > 1) YVolateAndCn.RemoveAt(YVolateAndCn.Count - 1);
            if (YVolateAndCn2.Count > 1) YVolateAndCn2.RemoveAt(YVolateAndCn2.Count - 1);
            if (YVolateAndCn3.Count > 1) YVolateAndCn3.RemoveAt(YVolateAndCn3.Count - 1);
            if (YVolateAndCn4.Count > 1) YVolateAndCn4.RemoveAt(YVolateAndCn4.Count - 1);
            if (XFre.Count > 1) XFre.RemoveAt(XFre.Count - 1);
            if (YVolateAndCurrent.Count > 1) YVolateAndCurrent.RemoveAt(YVolateAndCurrent.Count - 1);
            if (YVolateAndCurrent2.Count > 1) YVolateAndCurrent2.RemoveAt(YVolateAndCurrent2.Count - 1);
            if (YVolateAndCurrent3.Count > 1) YVolateAndCurrent3.RemoveAt(YVolateAndCurrent3.Count - 1);
            if (YVolateAndCurrent4.Count > 1) YVolateAndCurrent4.RemoveAt(YVolateAndCurrent4.Count - 1);
            if (XTime.Count > 1) XTime.RemoveAt(XTime.Count - 1);
            if (YVolateAndTan.Count > 1) YVolateAndTan.RemoveAt(YVolateAndTan.Count - 1);
            if (YVolateAndTan2.Count > 1) YVolateAndTan2.RemoveAt(YVolateAndTan2.Count - 1);
            if (YVolateAndTan3.Count > 1) YVolateAndTan3.RemoveAt(YVolateAndTan3.Count - 1);
            if (YVolateAndTan4.Count > 1) YVolateAndTan4.RemoveAt(YVolateAndTan4.Count - 1);
            UpdataWaveForm();

        }

        private void UpdataWaveForm()
        {
            #region
            if (Com1.SelectedIndex == 0)
            {

                if (Com2.SelectedIndex == 0)
                {
                    mv.SetChartObserver(YVolateAndCn.ToArray(), XVolate, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCn2.ToArray(), XVolate, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCn3.ToArray(), XVolate, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCn4.ToArray(), XVolate, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 1)
                {
                    mv.SetChartObserver(YVolateAndCurrent.ToArray(), XVolate, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCurrent2.ToArray(), XVolate, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCurrent3.ToArray(), XVolate, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCurrent4.ToArray(), XVolate, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 2)
                {
                    mv.SetChartObserver(YVolateAndTan.ToArray(), XVolate, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndTan2.ToArray(), XVolate, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndTan3.ToArray(), XVolate, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndTan4.ToArray(), XVolate, ChartPannel.Channel4);

                }

            }
            if (Com1.SelectedIndex == 1)
            {
                if (Com2.SelectedIndex == 0)
                {
                    mv.SetChartObserver(YVolateAndCn.ToArray(), XFre, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCn2.ToArray(), XFre, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCn3.ToArray(), XFre, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCn4.ToArray(), XFre, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 1)
                {
                    mv.SetChartObserver(YVolateAndCurrent.ToArray(), XFre, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCurrent2.ToArray(), XFre, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCurrent3.ToArray(), XFre, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCurrent4.ToArray(), XFre, ChartPannel.Channel4);

                }
                else
                {
                    mv.SetChartObserver(YVolateAndTan.ToArray(), XFre, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndTan2.ToArray(), XFre, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndTan3.ToArray(), XFre, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndTan4.ToArray(), XFre, ChartPannel.Channel4);

                }
            }
            if (Com1.SelectedIndex == 2)
            {
                if (Com2.SelectedIndex == 0)
                {
                    mv.SetChartObserver(YVolateAndCn.ToArray(), XTime, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCn2.ToArray(), XTime, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCn3.ToArray(), XTime, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCn4.ToArray(), XTime, ChartPannel.Channel4);

                }
                else if (Com2.SelectedIndex == 1)
                {
                    mv.SetChartObserver(YVolateAndCurrent.ToArray(), XTime, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndCurrent2.ToArray(), XTime, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndCurrent3.ToArray(), XTime, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndCurrent4.ToArray(), XTime, ChartPannel.Channel4);

                }
                else
                {
                    mv.SetChartObserver(YVolateAndTan.ToArray(), XTime, ChartPannel.Channel1);
                    mv.SetChartObserver(YVolateAndTan2.ToArray(), XTime, ChartPannel.Channel2);
                    mv.SetChartObserver(YVolateAndTan3.ToArray(), XTime, ChartPannel.Channel3);
                    mv.SetChartObserver(YVolateAndTan4.ToArray(), XTime, ChartPannel.Channel4);

                }
            }
            #endregion
            // Recor_Click(null, null);
        }

        public bool VolateState { get; set; } = false;
        private void Power(object sender, RoutedEventArgs e)
        {
            NeedVolate = 0;
            if (PowerState.IsChecked != false)
            {
                mv.StartPower();
                mv.IsEnable = true;
            }
            else
            {
                mv.ClosePower();
            }
            #region enable
            if (PowerState.IsChecked == false)
            {
                b1.IsEnabled = false;
                b2.IsEnabled = false;
                b3.IsEnabled = false;
                b4.IsEnabled = false;
                b5.IsEnabled = false;
                b6.IsEnabled = false;
                b7.IsEnabled = false;
                b8.IsEnabled = false;
                FreGroupBox.IsEnabled = false;
            }
            else
            {
                b1.IsEnabled = true;
                b2.IsEnabled = true;
                b3.IsEnabled = true;
                b4.IsEnabled = true;
                b5.IsEnabled = true;
                b6.IsEnabled = true;
                b7.IsEnabled = true;
                b8.IsEnabled = true;
                FreGroupBox.IsEnabled = true;
            }
            #endregion
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DownVolate();
        }

        private void Add_10K__Val_Click(object sender, RoutedEventArgs e)
        {
            AddVolate(10000);
            // IsFasle(VolateGroupBox, 2000);

        }

        private void IsFasle(Control btn, int Timer)
        {
            btn.IsEnabled = false;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = Timer;
            timer.Elapsed += delegate
            {
                btn.Dispatcher.Invoke(new Action(delegate
                {
                    btn.IsEnabled = true;
                }));
                timer.Stop();
            };
            timer.Start();
        }

        private void Add_1K__Val_Click(object sender, RoutedEventArgs e)
        {
            AddVolate(1000);
            // IsFasle(VolateGroupBox, 2000);

        }

        private void Add_100__Val_Click(object sender, RoutedEventArgs e)
        {
            AddVolate(300);
            // IsFasle(VolateGroupBox, 2000);

        }

        private void Add_10__Val_Click(object sender, RoutedEventArgs e)
        {
            AddVolate(100);
            //IsFasle(VolateGroupBox, 2000);v

        }

        private void Mul_10K_Vol_CLick(object sender, RoutedEventArgs e)
        {
            MulVolate(10000);
            // IsFasle(VolateGroupBox, 2000);
        }

        private void Mul_1K_Vol_CLick(object sender, RoutedEventArgs e)
        {
            MulVolate(1000);
            // IsFasle(VolateGroupBox, 2000);
        }

        private void Mul_100_Vol_CLick(object sender, RoutedEventArgs e)
        {
            MulVolate(300);
            //IsFasle(VolateGroupBox, 2000);
        }

        private void Mul_10_Vol_CLick(object sender, RoutedEventArgs e)
        {
            MulVolate(100);
            // IsFasle(VolateGroupBox, 2000);
        }

        private void AddVolate(float addbum)
        {

            NeedVolate += addbum;
            mv.SetBaseVolate(NeedVolate);
            NeedVolateText.Text = NeedVolate.ToString();
            Thread.Sleep(300);
            mv.UpVolate();
        }
        private void MulVolate(float addbum)
        {
            //  var tempdata = NumericsConverter.Text2Value(mv.HVVoltage.ToString());
            // var tempdata = NumericsConverter.Text2Value("100kV");
            if (NeedVolate >= addbum)
            {
                NeedVolate -= addbum;
                mv.SetBaseVolate(NeedVolate);
                NeedVolateText.Text = NeedVolate.ToString();

                Thread.Sleep(300);
                mv.UpVolate();
            }
            else
            {
                NeedVolate = 0;
                mv.SetBaseVolate(NeedVolate);
                NeedVolateText.Text = NeedVolate.ToString(); ;
                Thread.Sleep(300);
                mv.UpVolate();
            }

        }

        private void DownVolate()
        {
            mv.DownVolate();
        }
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            // PowerState.IsChecked = false;

            NeedVolate = 0;
            DownVolate();

        }

        private void SaveImageLocal()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "BMP|*.bmp|PNG|*.png|JPG|*.jpg";

            if (save.ShowDialog().Value)
            {
                if (SaveImg(save.FileName))
                {
                    MessageBox.Show("保存成功!");
                }
            }
        }

        private void SaveImageRemote(string imagePath, string imagename)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "BMP|*.bmp|PNG|*.png|JPG|*.jpg";
            SaveImg(imagePath + "\\" + imagename);
        }

        private bool SaveImg(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)ChartCollection.ActualWidth,  //ic是控件的名字
                    (int)ChartCollection.ActualHeight + 100, 1 / 48, 1 / 48, PixelFormats.Pbgra32);
                bmp.Render(ChartCollection);
                BitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void Keyb_OutCnData(string cn)
        {
            //CnTbx.Text = cn;
            //CnButton.Content = cn;
            //PhysicalVariable need = NumericsConverter.Text2Value(cn);
            //mv.Cn = need;//原 不转换为need

        }

        private void Cn_Click(object sender, RoutedEventArgs e)
        {

            //Views.KeyBoard keyb = new Views.KeyBoard();
            //keyb.OutCnData += Keyb_OutCnData;
            //keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //keyb.ShowDialog();
        }

        private void CnTan_Click(object sender, RoutedEventArgs e)
        {
            //Views.CnTanKeyBord keyb = new Views.CnTanKeyBord();
            //keyb.OutCnTanData += Keyb_OutCnTanData;
            //keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //keyb.ShowDialog();
        }

        private void Keyb_OutCnTanData(string cn)
        {
            //CnTanTxb.Text = cn;
            //CnTanButton.Content = cn;
            //PhysicalVariable need = NumericsConverter.Text2Value(cn);
            //mv.AGn = need;

        }
        private void X_selectChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Com2.SelectedIndex == 0)
                    mv.YPopu = "Volate";
                if (Com2.SelectedIndex == 1)
                    mv.YPopu = "Captance";
                if (Com2.SelectedIndex == 2)
                    mv.YPopu = "Tan";
                UpdataWaveForm();
            }
            catch
            {
            }
        }

        private void Y_selectchanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Com2.SelectedIndex == 0)
                    mv.YPopu = "Volate";
                if (Com2.SelectedIndex == 1)
                    mv.YPopu = "Captance";
                if (Com2.SelectedIndex == 2)
                    mv.YPopu = "Tan";
                UpdataWaveForm();
            }
            catch
            {
            }
        }

        private void Setting_click(object sender, RoutedEventArgs e)
        {
            Views.Setting setting = new Views.Setting();
            setting.ShowDialog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveImageLocal();
        }

        private void Project_click(object sender, RoutedEventArgs e)
        {
            Views.ProjectManagement pj = new Views.ProjectManagement();
            pj.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            pj.ShowDialog();
        }
        private void Main_load(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                this.DataContext = DataContext as MainWindowModel;
            }
        }

        private void unload_load(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
        }
    }
}
