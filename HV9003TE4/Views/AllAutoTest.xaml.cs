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
        MainWindowModel mv;
        public AllAutoTest()
        {
            InitializeComponent();
            mv = new MainWindowModel();
            this.DataContext = mv;
            mv.MyPAllAutoTestOrOrdeTestroperty = true;//只提供回复测量结果功能
            mv.StartTcp();
            try
            {
                mv.StartRecCom();
                mv.SetFre(50);
                mv.T1.IsBackground = true;
                mv.T1.Start();
            }
            catch
            {
                mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
        }
        public AllAutoTest(byte[] temp)
        {
            InitializeComponent();
            mv = new MainWindowModel();
            this.DataContext = mv;
            mv.MyPAllAutoTestOrOrdeTestroperty = true;//只提供回复测量结果功能
            mv.IsTcpTestting = true;//
            mv.SysData = temp;
            mv.AllTestResult.Clear();
            Models.SysAutoTestResult sys = Models.StaticClass.GetDataForTcpAutoTest(temp);
            mv.StartTcp();
            try
            {
                mv.StartRecCom();
                mv.SetFre(50);
                mv.T1.IsBackground = true;
                mv.T1.Start();
            }
            catch
            {
                mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
            sys1 = sys;
            Bitconvert(sys);
            mv.StartAutobytcp();

        }
        public AllAutoTest(Models.SysAutoTestResult temp)
        {
            InitializeComponent();
            mv = new MainWindowModel();
            this.DataContext = mv;
            mv.AllTestResult.Clear();
            mv.SysProject = temp;
            mv.StartTcp();
            try
            {
                mv.StartRecCom();
                mv.SetFre(50);
                mv.T1.IsBackground = true;
                mv.T1.Start();
            }
            catch
            {
                mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
            sys1 = temp;
            Bitconvert(temp);
            mv.StartAutobyProject();
        }

        /// <summary>
        ///  mv.ListboxItemsources数据源添加
        /// </summary>
        /// <param name="sys"></param>
        private void Bitconvert(Models.SysAutoTestResult sys)
        {
            foreach (var a in sys.NeedTestList)
            {
                mv.ListboxItemsources.Add("待测电压  :" + a.ToString() + "V");
            }
            mv.ListboxItemsources.Add("待测电晕  :" + sys.EleY.ToString() + "V");
            mv.ListboxItemsources.Add("持续时间  :" + sys.HideTime.ToString() + ":" + "耐   压 :" + sys.EleVolate.ToString());
            ISHAVEVOLATE = true;
            ISHAVEDY = true;
            time.Text = sys.HideTime.ToString();
            Expander.IsExpanded = true;
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
                    string[] a = mv.ListboxItemsources.ToArray();
                    mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    mv.ListboxItemsources.Add("待测电压  :" + need);
                    mv.ListboxItemsources.Add(a[a.Length - 1]);
                }
                else
                {
                    string[] a = mv.ListboxItemsources.ToArray();
                    mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);
                    mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    mv.ListboxItemsources.Add("待测电压  :" + need);
                    mv.ListboxItemsources.Add(a[a.Length - 2]);
                    mv.ListboxItemsources.Add(a[a.Length - 1]);
                }

            }
            else
            {
                PhysicalVariable need = NumericsConverter.Text2Value(cn);
                mv.ListboxItemsources.Add("待测电压  :" + need);
            }


        }

        private void Mul_Click(object sender, RoutedEventArgs e)
        {
            if (mv.ListboxItemsources.Count != 0)
                mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);

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
                mv.ListboxItemsources.Add("待测电晕  :" + need);
                ISHAVEDY = true;
            }
            else
            {
                if (ISHAVEVOLATE)
                {
                    string[] b = mv.ListboxItemsources.ToArray();
                    mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);
                    mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    mv.ListboxItemsources.Add("待测电晕  :" + need);
                    mv.ListboxItemsources.Add(b[b.Length - 1]);
                }
                else
                {
                    mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    mv.ListboxItemsources.Add("待测电晕  :" + need);
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
                    mv.ListboxItemsources.Add("持续时间  :" + time.Text + ":" + "  耐    压:" + need);
                    ISHAVEVOLATE = true;
                }
                else
                {
                    if (time.Text == "")
                        time.Text = "60";
                    mv.ListboxItemsources.RemoveAt(mv.ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    mv.ListboxItemsources.Add("持续时间  :" + time.Text + ":" + "耐   压 :" + need);
                }
            }
        }
        private void StartTest_click(object sender, RoutedEventArgs e)
        {
            Expander.IsExpanded = false;
            if (ISHAVEDY)
            {
                if (ISHAVEVOLATE)
                {
                    if (mv.ListboxItemsources.Count != 0)
                    {
                        mv.StartAuto();
                    }
                    else
                    {
                        mv.ShowHide("请添加测量数据");
                    }
                }
                else
                {
                    mv.ShowHide("请添加耐压电压");
                }
            }
            else
            {
                mv.ShowHide("请添加电晕测量电压");
            }
        }

        private void Stop_click(object sender, RoutedEventArgs e)
        {
            mv.ResetTest();
        }

        private void Continur_click(object sender, RoutedEventArgs e)
        {
            mv.ContinuTest();
        }
        private void Quatity_Click(object sender, RoutedEventArgs e)
        {
            mv.QuqlityIsOk = Visibility.Collapsed;
            Models.AutoStateStatic.SState.Quality = true;
            Models.AutoStateStatic.SState.IsPress = true;
            Models.StaticClass.SaveImageRemote("C:\\WaveImage", "1.jpg", EleyChart);
            Task.Factory.StartNew(SaveVolateImage);
        }
        private void SaveVolateImage()
        {
            while (Models.AutoStateStatic.SState.CompeleteVolate)
            {
                Models.StaticClass.SaveImageRemote("C:\\WaveImage", "1.jpg", Volatechart);
                Thread.Sleep(500);
            }
        }


        private void QualityNot_Click(object sender, RoutedEventArgs e)
        {
            mv.QuqlityIsOk = Visibility.Collapsed;
            Models.AutoStateStatic.SState.Quality = false;
            Models.AutoStateStatic.SState.IsPress = true;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            // mv.AllAutoTestIsOpen = false;
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
            mv.ResetTest();
        }
    }
}
