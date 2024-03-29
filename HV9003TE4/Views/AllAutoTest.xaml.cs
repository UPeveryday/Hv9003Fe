﻿using System;
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
using SCEEC.NET.TCPSERVER;
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
            mv.MyPAllAutoTestOrOrdeTestroperty = true;//只提供回复测量结果功能
            TcpTask.TcpServer.CloseAllClient();
            mv.MulStartTcp();
            mv.StartTcp();
            try
            {
                mv.StartRecCom();
                mv.CreateFourTan();//实例化对象，临时
                mv.CreateTanEleVolate();//实例化对象，临时
            }
            catch
            {
                mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
            saveImagge();
            this.DataContext = mv;
        }
        public AllAutoTest(byte[] temp)
        {
            InitializeComponent();
            mv = new MainWindowModel();
            this.DataContext = mv;
            mv.MyPAllAutoTestOrOrdeTestroperty = true;//只提供回复测量结果功能
            Models.StaticClass.IsTcpTestting = true;
            // mv.IsTcpTestting = true;//
            mv.SysData = temp;
            Models.StaticClass.AllTestResult.Clear();
            // mv.AllTestResult.Clear();
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
            saveImagge();

        }
        public AllAutoTest(Models.SysAutoTestResult temp)
        {
            InitializeComponent();
            mv = new MainWindowModel();
            Models.StaticClass.AllTestResult.Clear();
            mv.SysProject = temp;
            mv.StartTcp();
            try
            {
                mv.StartRecCom();
                // mv.SetFre(50);
                //mv.T1.IsBackground = true;
                //mv.T1.Start();
                mv.CreateFourTan();//实例化对象，临时
                mv.CreateTanEleVolate();//实例化对象，临时
            }
            catch
            {
                mv.ShowHide("初始化程序发生错误" + "\r\n" + "请检查串口及仪器连接");
            }
            sys1 = temp;
            Bitconvert(temp);
            //  mv.StartAutobyProject();
            saveImagge();
            this.DataContext = null;
            this.DataContext = mv;
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
            if (ISHAVEDY)
            {
                if (ISHAVEVOLATE)
                {
                    if (mv.ListboxItemsources.Count != 0)
                    {
                        mv.InitTest();
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
            // mv.ResetTest();
            mv.StartPower();

        }

        private void Continur_click(object sender, RoutedEventArgs e)
        {
            // mv.ContinuTest();
            // mv.ClosePower();
            mv.REset();


        }
        private void Quatity_Click(object sender, RoutedEventArgs e)
        {
            mv.QuqlityIsOk = Visibility.Collapsed;
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
        public void saveImagge()
        {
            Task.Factory.StartNew(() =>
            {
                savecapatanceimage(chartp1,chartp2, chartp3, chartp4);
            });
        }

        private void savecapatanceimage(Control control1, Control control2, Control control3, Control control4)
        {
            while (Models.AutoStateStatic.SState.CaptanceCompelete)
            {
                Models.StaticClass.SaveImageRemote("C:\\WaveImage", "1.jpg", control1);
                Models.StaticClass.SaveImageRemote("C:\\WaveImage", "2.jpg", control2);
                Models.StaticClass.SaveImageRemote("C:\\WaveImage", "3.jpg", control3);
                Models.StaticClass.SaveImageRemote("C:\\WaveImage", "4.jpg", control4);
                Models.AutoStateStatic.SState.CaptanceCompelete = false;
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                mv.MulStartTcp();
                Models.AutoStateStatic.SState.vm.StartTcp();
            });

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
            mv.CancerTest();
            GC.Collect();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Models.AutoStateStatic.SState.IsOPenAuto = false;
            mv.DownVolate();
            mv.ResetTest();
            mv.CancerTest();
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmb.SelectedIndex == 0)
                    mv.SetFre(50);
                else
                    mv.SetFre(60);
            }
            catch
            {
            }


        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            mv.REset();
        }

        private void StartPower(object sender, RoutedEventArgs e)
        {
            mv.StartPower();
        }

        private void ClosePower(object sender, RoutedEventArgs e)
        {
            mv.ClosePower();
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mv.FontSize = (ushort)(ActualHeight / 600 * 15);
        }

        private void Time_KeyDown(object sender, KeyEventArgs e)
        {

            //if(e.Key==Key.D0|| e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || 
            //    e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9||
            //    e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 ||
            //    e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9)
            //{

            //}
            //else
            //{
            //    time.Text = "";
            //    MessageBox.Show("必须为正整数");
            //}
        }

        private void Expander_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mv.ISREMOTE == true)
                mv.ISREMOTE = false;
            else
                mv.ISREMOTE = true;

        }
    }
}
