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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using SCEEC.Numerics;

namespace HV9003TE4.Views
{
    /// <summary>
    /// AllAutoTest.xaml 的交互逻辑
    /// </summary>
    public partial class AllAutoTest : MetroWindow
    {
        public Models.SysAutoTestResult sys1 { get; set; }
        // MainWindowModel Models.AutoStateStatic.SState.mv;
        public AllAutoTest()
        {
            InitializeComponent();
            Models.AutoStateStatic.SState.mv = new MainWindowModel();
            this.DataContext = Models.AutoStateStatic.SState.mv;
            Models.AutoStateStatic.SState.mv.MyPAllAutoTestOrOrdeTestroperty = true;//只提供回复测量结果功能
            Models.AutoStateStatic.SState.mv.StartTcp();
            try
            {
                Models.AutoStateStatic.SState.mv.StartRecCom();
                // Models.AutoStateStatic.SState.mv.SetFre(50);
                Models.AutoStateStatic.SState.mv.T1.IsBackground = true;
                Models.AutoStateStatic.SState.mv.T1.Start();
                Models.AutoStateStatic.SState.mv.CreateFourTan();//实例化对象，临时
                Models.AutoStateStatic.SState.mv.CreateTanEleVolate();//实例化对象，临时
            }
            catch
            {
                Models.AutoStateStatic.SState.mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
        }
        public AllAutoTest(byte[] temp)
        {
            InitializeComponent();
            Models.AutoStateStatic.SState.mv = new MainWindowModel();
            this.DataContext = Models.AutoStateStatic.SState.mv;
            Models.AutoStateStatic.SState.mv.MyPAllAutoTestOrOrdeTestroperty = true;//只提供回复测量结果功能
            Models.StaticClass.IsTcpTestting = true;
            // Models.AutoStateStatic.SState.mv.IsTcpTestting = true;//
            Models.AutoStateStatic.SState.mv.SysData = temp;
            Models.StaticClass.AllTestResult.Clear();
            // Models.AutoStateStatic.SState.mv.AllTestResult.Clear();
            Models.SysAutoTestResult sys = Models.StaticClass.GetDataForTcpAutoTest(temp);
            Models.AutoStateStatic.SState.mv.StartTcp();
            try
            {
                Models.AutoStateStatic.SState.mv.StartRecCom();
                Models.AutoStateStatic.SState.mv.SetFre(50);
                Models.AutoStateStatic.SState.mv.T1.IsBackground = true;
                Models.AutoStateStatic.SState.mv.T1.Start();
            }
            catch
            {
                Models.AutoStateStatic.SState.mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
            sys1 = sys;
            Bitconvert(sys);
            Models.AutoStateStatic.SState.mv.StartAutobytcp();

        }
        public AllAutoTest(Models.SysAutoTestResult temp)
        {
            InitializeComponent();
            Models.AutoStateStatic.SState.mv = new MainWindowModel();
            Models.StaticClass.AllTestResult.Clear();
            Models.AutoStateStatic.SState.mv.SysProject = temp;
            Models.AutoStateStatic.SState.mv.StartTcp();
            try
            {
                Models.AutoStateStatic.SState.mv.StartRecCom();
                // Models.AutoStateStatic.SState.mv.SetFre(50);
                //Models.AutoStateStatic.SState.mv.T1.IsBackground = true;
                //Models.AutoStateStatic.SState.mv.T1.Start();
                Models.AutoStateStatic.SState.mv.CreateFourTan();//实例化对象，临时
                Models.AutoStateStatic.SState.mv.CreateTanEleVolate();//实例化对象，临时
            }
            catch
            {
                Models.AutoStateStatic.SState.mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
            sys1 = temp;
            Bitconvert(temp);
            //  Models.AutoStateStatic.SState.mv.StartAutobyProject();
            this.DataContext = null;
            this.DataContext = Models.AutoStateStatic.SState.mv;
        }

        /// <summary>
        ///  Models.AutoStateStatic.SState.mv.ListboxItemsources数据源添加
        /// </summary>
        /// <param name="sys"></param>
        private void Bitconvert(Models.SysAutoTestResult sys)
        {
            foreach (var a in sys.NeedTestList)
            {
                Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电压  :" + a.ToString() + "V");
            }
            Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电晕  :" + sys.EleY.ToString() + "V");
            Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("持续时间  :" + sys.HideTime.ToString() + ":" + "耐   压 :" + sys.EleVolate.ToString());
            ISHAVEVOLATE = true;
            ISHAVEDY = true;
            time.Text = sys.HideTime.ToString();
            //Expander.IsExpanded = true;
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Max_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Views.AddVlate keyb = new Views.AddVlate(1);
            keyb.OutCnData += Keyb_OutCnData;
            keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            keyb.ShowDialog();
        }

        private void Keyb_OutCnData(string cn)
        {
            if (ISHAVEDY)
            {
                if (!ISHAVEVOLATE)
                {
                    string[] a = Models.AutoStateStatic.SState.mv.ListboxItemsources.ToArray();
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电压  :" + need);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add(a[a.Length - 1]);
                }
                else
                {
                    string[] a = Models.AutoStateStatic.SState.mv.ListboxItemsources.ToArray();
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电压  :" + need);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add(a[a.Length - 2]);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add(a[a.Length - 1]);
                }

            }
            else
            {
                PhysicalVariable need = NumericsConverter.Text2Value(cn);
                Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电压  :" + need);
            }


        }

        private void Mul_Click(object sender, RoutedEventArgs e)
        {
            if (Models.AutoStateStatic.SState.mv.ListboxItemsources.Count != 0)
                Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);

        }
        private bool ISHAVEVOLATE = false;
        private bool ISHAVEDY = false;
        private void SetY(object sender, RoutedEventArgs e)
        {
            Views.AddVlate keyb = new Views.AddVlate(3);
            keyb.OutDYData += Keyb_OutDYData;
            keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            keyb.ShowDialog();
        }

        // private bool ISHAVEVOLATE = false;
        private void Keyb_OutDYData(string cn)
        {
            if (!ISHAVEDY)
            {

                PhysicalVariable need = NumericsConverter.Text2Value(cn);
                Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电晕  :" + need);
                ISHAVEDY = true;
            }
            else
            {
                if (ISHAVEVOLATE)
                {
                    string[] b = Models.AutoStateStatic.SState.mv.ListboxItemsources.ToArray();
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电晕  :" + need);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add(b[b.Length - 1]);
                }
                else
                {
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("待测电晕  :" + need);
                }
            }

        }

        private void SetVolate(object sender, RoutedEventArgs e)
        {
            Views.AddVlate keyb = new Views.AddVlate(2);
            keyb.OutVolateData += Keyb_OutVolateData;
            keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            keyb.ShowDialog();
        }

        private void Keyb_OutVolateData(string cn)
        {
            if (ISHAVEDY)
            {
                if (!ISHAVEVOLATE)
                {
                    if (time.Text == "")
                        time.Text = "60";
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("持续时间  :" + time.Text + ":" + "  耐    压:" + need);
                    ISHAVEVOLATE = true;
                }
                else
                {
                    if (time.Text == "")
                        time.Text = "60";
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.RemoveAt(Models.AutoStateStatic.SState.mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    Models.AutoStateStatic.SState.mv.ListboxItemsources.Add("持续时间  :" + time.Text + ":" + "耐   压 :" + need);
                }
            }
        }
        private void StartTest_click(object sender, RoutedEventArgs e)
        {
            // Expander.IsExpanded = false;
            if (ISHAVEDY)
            {
                if (ISHAVEVOLATE)
                {
                    if (Models.AutoStateStatic.SState.mv.ListboxItemsources.Count != 0)
                    {
                        Models.AutoStateStatic.SState.mv.InitTest();
                        Models.AutoStateStatic.SState.mv.StartAuto();
                    }
                    else
                    {
                        Models.AutoStateStatic.SState.mv.ShowHide("请添加测量数据");
                    }
                }
                else
                {
                    Models.AutoStateStatic.SState.mv.ShowHide("请添加耐压电压");
                }
            }
            else
            {
                Models.AutoStateStatic.SState.mv.ShowHide("请添加电晕测量电压");
            }
        }

        private void Stop_click(object sender, RoutedEventArgs e)
        {
            // Models.AutoStateStatic.SState.mv.ResetTest();
            Models.AutoStateStatic.SState.mv.StartPower();

        }

        private void Continur_click(object sender, RoutedEventArgs e)
        {
            // Models.AutoStateStatic.SState.mv.ContinuTest();
            // Models.AutoStateStatic.SState.mv.ClosePower();
            Models.AutoStateStatic.SState.mv.REset();


        }
        private void Quatity_Click(object sender, RoutedEventArgs e)
        {
            Models.AutoStateStatic.SState.mv.QuqlityIsOk = Visibility.Collapsed;
            Models.AutoStateStatic.SState.Quality = true;
            Models.AutoStateStatic.SState.IsPress = true;
            //  Models.StaticClass.SaveImageRemote("C:\\WaveImage", "1.jpg", EleyChart);
            Task.Factory.StartNew(SaveVolateImage);
        }
        private void SaveVolateImage()
        {
            while (Models.AutoStateStatic.SState.CompeleteVolate)
            {
                //  Models.StaticClass.SaveImageRemote("C:\\WaveImage", "1.jpg", Volatechart);
                Thread.Sleep(500);
            }
        }


        private void QualityNot_Click(object sender, RoutedEventArgs e)
        {
            Models.AutoStateStatic.SState.mv.QuqlityIsOk = Visibility.Collapsed;
            Models.AutoStateStatic.SState.Quality = false;
            Models.AutoStateStatic.SState.IsPress = true;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            // Models.AutoStateStatic.SState.mv.AllAutoTestIsOpen = false;
        }

        private void all_load(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                this.DataContext = DataContext as MainWindowModel;
            }
            this.DataContext = DataContext as MainWindowModel;

        }

        private void Undload(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
        }

        ~AllAutoTest()
        {
            Models.AutoStateStatic.SState.mv.ResetTest();
            Models.AutoStateStatic.SState.mv.CancerTest();
            GC.Collect();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Models.AutoStateStatic.SState.mv.DownVolate();
            //Models.AutoStateStatic.SState.mv.ResetTest();
            //Models.AutoStateStatic.SState.mv.CancerTest();
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmb.SelectedIndex == 0)
                    Models.AutoStateStatic.SState.mv.SetFre(50);
                else
                    Models.AutoStateStatic.SState.mv.SetFre(60);
            }
            catch
            {
            }


        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            Models.AutoStateStatic.SState.mv.REset();
        }

        private void StartPower(object sender, RoutedEventArgs e)
        {
            Models.AutoStateStatic.SState.mv.StartPower();
        }

        private void ClosePower(object sender, RoutedEventArgs e)
        {
            Models.AutoStateStatic.SState.mv.ClosePower();
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Models.AutoStateStatic.SState.mv.FontSize = (ushort)(ActualHeight / 600 * 15);
        }


    }
}
