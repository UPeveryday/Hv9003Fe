using System;
using System.Collections.Generic;
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
using SCEEC.Numerics;

namespace HV9003TE4.Views
{
    /// <summary>
    /// AllAutoTest.xaml 的交互逻辑
    /// </summary>
    public partial class AllAutoTest : MetroWindow
    {
        public AllAutoTest()
        {
            InitializeComponent();
            (this.DataContext as MainWindowModel).StartTcp();
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
                    string[] a = (this.DataContext as MainWindowModel).ListboxItemsources.ToArray();
                    (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add("待测电压  :" + need);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add(a[a.Length - 1]);
                }
                else
                {
                    string[] a = (this.DataContext as MainWindowModel).ListboxItemsources.ToArray();
                    (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);
                    (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add("待测电压  :" + need);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add(a[a.Length - 2]);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add(a[a.Length - 1]);
                }

            }
            else
            {
                PhysicalVariable need = NumericsConverter.Text2Value(cn);
                (this.DataContext as MainWindowModel).ListboxItemsources.Add("待测电压  :" + need);
            }


        }

        private void Mul_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as MainWindowModel).ListboxItemsources.Count != 0)
                (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);

        }

        private void SetY(object sender, RoutedEventArgs e)
        {
            Views.AddVlate keyb = new Views.AddVlate(3);
            keyb.OutDYData += Keyb_OutDYData;
            keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            keyb.ShowDialog();
        }
        private bool ISHAVEVOLATE = false;
        private bool ISHAVEDY = false;
        // private bool ISHAVEVOLATE = false;
        private void Keyb_OutDYData(string cn)
        {
            if (!ISHAVEDY)
            {

                PhysicalVariable need = NumericsConverter.Text2Value(cn);
                (this.DataContext as MainWindowModel).ListboxItemsources.Add("待测电晕  :" + need);
                ISHAVEDY = true;
            }
            else
            {
                if (ISHAVEVOLATE)
                {
                    string[] b = (this.DataContext as MainWindowModel).ListboxItemsources.ToArray();
                    (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);
                    (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add("待测电晕  :" + need);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add(b[b.Length - 1]);
                }
                else
                {
                    (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add("待测电晕  :" + need);
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
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add("持续时间  :" + time.Text + ":" + "  耐    压:" + need);
                    ISHAVEVOLATE = true;
                }
                else
                {
                    if (time.Text == "")
                        time.Text = "60";
                    (this.DataContext as MainWindowModel).ListboxItemsources.RemoveAt((this.DataContext as MainWindowModel).ListboxItemsources.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ListboxItemsources.Add("持续时间  :" + time.Text + ":" + "耐   压 :" + need);
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
                    if ((this.DataContext as MainWindowModel).ListboxItemsources.Count != 0)
                    {
                        (this.DataContext as MainWindowModel).StartAuto();
                    }
                    else
                    {
                        (this.DataContext as MainWindowModel).ShowHide("请添加测量数据");
                    }
                }
                else
                {
                    (this.DataContext as MainWindowModel).ShowHide("请添加耐压电压");
                }
            }
            else
            {
                (this.DataContext as MainWindowModel).ShowHide("请添加电晕测量电压");
            }
        }

        private void Stop_click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainWindowModel).ResetTest();
        }

        private void Continur_click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainWindowModel).ContinuTest();
        }
        private void Quatity_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainWindowModel).QuqlityIsOk = Visibility.Collapsed;
            Models.AutoStateStatic.SState.Quality = true;
            Models.AutoStateStatic.SState.IsPress = true;
        }
        private void QualityNot_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainWindowModel).QuqlityIsOk = Visibility.Collapsed;
            Models.AutoStateStatic.SState.Quality = false;
            Models.AutoStateStatic.SState.IsPress = true;
        }
    }
}
