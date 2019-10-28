using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using SCEEC.Numerics;

namespace HV9003TE4.Views
{
    /// <summary>
    /// ProjectManagement.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectManagement : MetroWindow
    {
        public ProjectManagement()
        {
            InitializeComponent();
            (this.DataContext as MainWindowModel).TimeSources.Clear();
            for (int i = 0; i < 100; i++)
            {
                (this.DataContext as MainWindowModel).TimeSources.Add(i);
            }
            Timecombobox.SelectedIndex = 60;
            InitPeoject(@"C:\PeojectIni");
        }

        private bool ISHAVEVOLATE = false;
        private bool ISHAVEDY = false;

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = ProjiectListBox.SelectedValue;
            if (a != null)
            {
                Addui(ReadIniFile("C:\\PeojectIni\\" + a.ToString() + ".ini"));
            }

        }
        public bool ShowProject { get; set; } = false;
        private void AddNewProject(object sender, RoutedEventArgs e)
        {
            //(this.DataContext as MainWindowModel).projiectnames.Add("addnewproject");
            if (!ShowProject)
            {
                GridProJect.Visibility = Visibility.Visible;
                ShowProject = true;
            }
            else
            {
                GridProJect.Visibility = Visibility.Collapsed;
                ShowProject = false;
            }

        }

        private void Addvolate_click(object sender, RoutedEventArgs e)
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
                    string[] a = (this.DataContext as MainWindowModel).ProjectVolate.ToArray();
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add("待测电压  :" + need);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add(a[a.Length - 1]);
                }
                else
                {
                    string[] a = (this.DataContext as MainWindowModel).ProjectVolate.ToArray();
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 1);
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add("待测电压  :" + need);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add(a[a.Length - 2]);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add(a[a.Length - 1]);
                }

            }
            else
            {
                PhysicalVariable need = NumericsConverter.Text2Value(cn);
                (this.DataContext as MainWindowModel).ProjectVolate.Add("待测电压  :" + need);
            }

        }



        private void Mulvolate(object sender, RoutedEventArgs e)
        {
            if (ISHAVEDY && !ISHAVEVOLATE)
            {
                if ((this.DataContext as MainWindowModel).ProjectVolate.Count > 1)
                {
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 2);

                }
            }
            if (ISHAVEVOLATE)
            {
                if ((this.DataContext as MainWindowModel).ProjectVolate.Count > 2)
                {
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 3);
                }
            }

        }

        private void SetY(object sender, RoutedEventArgs e)
        {
            Views.AddVlate keyb = new Views.AddVlate(3);
            keyb.OutDYData += Keyb_OutDYData;
            keyb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            keyb.ShowDialog();
        }

        private void Keyb_OutDYData(string cn)
        {
            if (!ISHAVEDY)
            {

                PhysicalVariable need = NumericsConverter.Text2Value(cn);
                (this.DataContext as MainWindowModel).ProjectVolate.Add("待测电晕  :" + need);
                ISHAVEDY = true;
            }
            else
            {
                if (ISHAVEVOLATE)
                {
                    string[] b = (this.DataContext as MainWindowModel).ProjectVolate.ToArray();
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 1);
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add("待测电晕  :" + need);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add(b[b.Length - 1]);
                }
                else
                {
                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add("待测电晕  :" + need);
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

                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add("持续时间:" + Timecombobox.SelectedIndex.ToString() + ":" + "  耐 压:" + need);
                    ISHAVEVOLATE = true;
                }
                else
                {

                    (this.DataContext as MainWindowModel).ProjectVolate.RemoveAt((this.DataContext as MainWindowModel).ProjectVolate.Count - 1);
                    PhysicalVariable need = NumericsConverter.Text2Value(cn);
                    (this.DataContext as MainWindowModel).ProjectVolate.Add("持续时间:" + Timecombobox.SelectedIndex.ToString() + ":" + "耐 压 :" + need);
                }
            }
        }
        public bool IsHavePname { get; set; } = false;
        private void projectname_lostfocus(object sender, RoutedEventArgs e)
        {

        }

        private void Save_project(object sender, RoutedEventArgs e)
        {
            if (ISHAVEVOLATE)
            {
                if (ISHAVEDY)
                {
                    if (!IsHavePname)
                    {
                        if (!File.Exists("C:\\PeojectIni"))
                        {
                            if (!File.Exists("C:\\PeojectIni\\" + projecttextbox.Text + ".ini"))
                            {
                                Models.WriteDataToFile.DeelDirectoryInfo("C:\\PeojectIni", Models.Mode.Create);
                                GetInidata("C:\\PeojectIni\\" + projecttextbox.Text + ".ini", Models.StaticClass.GetSys((this.DataContext as MainWindowModel).ProjectVolate));
                                (this.DataContext as MainWindowModel).projiectnames.Add(projecttextbox.Text);
                                IsHavePname = true;
                            }
                            else
                            {
                                ShowHide("已经存在此名称方案");
                                IsHavePname = true;
                            }
                        }
                    }
                    GridProJect.Visibility = Visibility.Collapsed;
                    InitProject();
                }
                else
                {
                    (this.DataContext as MainWindowModel).ShowHide("请设置电晕电压");
                }

            }
            else
            {
                (this.DataContext as MainWindowModel).ShowHide("请设置耐压电压");
            }

        }

        public void InitProject()
        {
            IsHavePname = false;
            ISHAVEDY = false;
            projecttextbox.Text = "";
            ISHAVEVOLATE = false;
            (this.DataContext as MainWindowModel).ProjectVolate.Clear();
        }
        /// <summary>
        /// 存储数据到ini文件
        /// </summary>
        /// <param name="path">.ini文件路径</param>
        /// <returns></returns>
        public bool GetInidata(string path, Models.SysAutoTestResult sysdata)
        {
            // sysdata = Models.StaticClass.GetSys((this.DataContext as MainWindowModel).ProjectVolate);
            try
            {
                float[] data = sysdata.NeedTestList.ToArray();
                int volateNum = data.Length;
            cc: if (File.Exists(path))
                {
                    Models.INIFiLE ini = new Models.INIFiLE(path);
                    ini.WriteInt("Project", "VolateNum", volateNum);
                    for (int i = 0; i < volateNum; i++)
                    {
                        ini.WriteString("Project", (i + 1).ToString(), data[i].ToString());
                    }
                    ini.WriteString("Project", "Corona", sysdata.EleY.ToString());
                    ini.WriteString("Project", "PressureVolate", sysdata.EleVolate.ToString());
                    ini.WriteInt("Project", "HodeTime", sysdata.HideTime);
                    return true;
                }
                else
                {
                    Models.WriteDataToFile.FileBaseDeel(path, Models.MyFileMode.Create);
                    goto cc;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 读取ini文件数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Models.SysAutoTestResult ReadIniFile(string path)
        {
            Models.SysAutoTestResult sys = new Models.SysAutoTestResult();
            if (File.Exists(path))
            {
                Models.INIFiLE ini = new Models.INIFiLE(path);
                int num = ini.ReadInt("Project", "VolateNum", 0);
                for (int i = 0; i < num; i++)
                {
                    sys.NeedTestList.Add((float)Convert.ToDouble(ini.ReadString("Project", (i + 1).ToString(), "")));
                }
                sys.EleY = (float)Convert.ToDouble(ini.ReadString("Project", "Corona", ""));
                sys.EleVolate = (float)Convert.ToDouble(ini.ReadString("Project", "PressureVolate", ""));
                sys.HideTime = ini.ReadInt("Project", "HodeTime", 0);
                sys.IsEleY = true;
                sys.IsVolate = true;
                return sys;
            }
            return null;
        }

        public void Addui(Models.SysAutoTestResult sys)
        {
            ObservableCollection<UIElement> uia = new ObservableCollection<UIElement>();
            int needyuinum = sys.NeedTestList.Count + 3;
            var data = sys.NeedTestList.ToArray();
            //if (sys.NeedTestList.Count != 1)
            //{

            for (int i = 0; i < sys.NeedTestList.Count / 3; i++)
            {
                uia.Add(TestVolateUi("测点:" + (i * 3 + 1).ToString(), "测点:" + (i * 3 + 2).ToString(), "测点:" + (i * 3 + 3).ToString(), data[i * 3].ToString(),
                    data[i * 3 + 1].ToString(), data[i * 3 + 2].ToString()));
            }
            int lostnum = sys.NeedTestList.Count % 3;
            if (lostnum == 1)
            {
                uia.Add(TestVolateUi("测点:" + (sys.NeedTestList.Count).ToString(), "电晕:", "耐压:",
                    data[sys.NeedTestList.Count - 1].ToString(), sys.EleY.ToString(), sys.EleVolate.ToString()));
                uia.Add(TestVolateUi("持续时间:", "", "", sys.HideTime.ToString(), "", ""));

            }
            if (lostnum == 2)
            {
                uia.Add(TestVolateUi("测点:" + (sys.NeedTestList.Count - 2 + 1).ToString(), "测点:" + (sys.NeedTestList.Count - 1 + 1).ToString(), "电晕:"
                    , data[sys.NeedTestList.Count - 2].ToString(), data[sys.NeedTestList.Count - 1].ToString(), sys.EleY.ToString()));
                uia.Add(TestVolateUi("耐压:", "持续时间:", "", sys.EleVolate.ToString(), sys.HideTime.ToString(), ""));

            }
            if (lostnum == 0)
            {
                uia.Add(TestVolateUi("电晕:", "耐压:", "持续时间:", sys.EleY.ToString(), sys.EleVolate.ToString(), sys.HideTime.ToString()));
            }
            //}
            //else
            //{

            //}
            (this.DataContext as MainWindowModel).Projectdataui = uia;

        }

        public UIElement TestVolateUi(string blc1, string blc2, string blc3, string tb1 = "", string tb2 = "", string tb3 = "")
        {
            Thickness th = new Thickness();
            th.Left = 5;
            th.Right = 5;
            th.Bottom = 5;
            th.Top = 5;
            DockPanel dock = new DockPanel();
            TextBlock block1 = new TextBlock();
            TextBox textBox1 = new TextBox();
            TextBlock block2 = new TextBlock();
            TextBox textBox2 = new TextBox();
            TextBlock block3 = new TextBlock();
            TextBox textBox3 = new TextBox();
            block1.Width = 50;
            block2.Width = 50;
            block3.Width = 50;
            block1.Width = 50;
            block2.Width = 50;
            block3.Width = 50;
            block1.Text = blc1;
            block2.Text = blc2;
            block3.Text = blc3;
            textBox1.Text = tb1;
            textBox2.Text = tb2;
            textBox3.Text = tb3;
            textBox1.MinWidth = 100;
            textBox2.MinWidth = 100;
            textBox3.MinWidth = 100;
            textBox1.MaxWidth = 200;
            textBox2.MaxWidth = 200;
            textBox3.MaxWidth = 200;
            block1.VerticalAlignment = VerticalAlignment.Center;
            block1.HorizontalAlignment = HorizontalAlignment.Center;
            block2.VerticalAlignment = VerticalAlignment.Center;
            block2.HorizontalAlignment = HorizontalAlignment.Center;
            block3.VerticalAlignment = VerticalAlignment.Center;
            block3.HorizontalAlignment = HorizontalAlignment.Center;
            textBox1.TextAlignment = TextAlignment.Right;
            textBox2.TextAlignment = TextAlignment.Right;
            textBox3.TextAlignment = TextAlignment.Right;
            textBox1.Margin = th;
            textBox2.Margin = th;
            textBox3.Margin = th;
            dock.Children.Add(block1);
            dock.Children.Add(textBox1);
            dock.Children.Add(block2);
            dock.Children.Add(textBox2);
            dock.Children.Add(block3);
            dock.Children.Add(textBox3);
            return dock;

        }
        /// <summary>
        /// 加载预制方案
        /// </summary>
        /// <param name="path"></param>
        public void InitPeoject(string path)
        {
            if (Directory.Exists(path))
            {
                string[] Inis = Directory.GetFiles(path, "*.ini");
                foreach (var a in Inis)
                {
                    var b = a.Split('\\');
                    (this.DataContext as MainWindowModel).projiectnames.Add(b[b.Length - 1].Replace(".ini", ""));
                }
            }
            else
            {
                Models.WriteDataToFile.DeelDirectoryInfo(path, Models.Mode.Create);

            }

        }


        public Models.SysAutoTestResult GetUiData(ListBox box)
        {
            Models.SysAutoTestResult sys = new Models.SysAutoTestResult();
            List<float> tpddata = new List<float>();
            if (box != null)
            {
                foreach (var a in box.Items.OfType<DockPanel>())
                {
                    foreach (var tb in a.Children.OfType<TextBox>())
                    {
                        if (tb.Text != "")
                        {
                            tpddata.Add((float)Convert.ToDouble(tb.Text));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                sys.HideTime = Convert.ToInt32(tpddata.ToArray()[tpddata.Count - 1]);
                sys.EleVolate = (float)Convert.ToDouble(tpddata.ToArray()[tpddata.Count - 2]);
                sys.EleY = (float)Convert.ToDouble(tpddata.ToArray()[tpddata.Count - 3]);
                sys.IsEleY = true;
                sys.IsVolate = true;
                tpddata.RemoveAt(tpddata.Count - 1);
                tpddata.RemoveAt(tpddata.Count - 1);
                tpddata.RemoveAt(tpddata.Count - 1);
                sys.NeedTestList = tpddata;
                return sys;
            }
            return null;
        }

        private void Saveproject(object sender, RoutedEventArgs e)
        {
            // ReadIniFile("C:\\PeojectIni\\测试一.ini");
            if (true == GetInidata("C:\\" + "PeojectIni\\" + ProjiectListBox.SelectedValue.ToString() + ".ini", GetUiData(resultlistbox)))
            {
                ShowHide("保存方案成功");
            }
            else
            {
                ShowHide("保存方案失败\t\n发生了未知的错误");
            }
        }
        private void ShowHide(string Text)
        {
            Alarm alarm = new Alarm(Text);
            alarm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            alarm.ShowDialog();
        }

        private void DeletePeoject(object sender, RoutedEventArgs e)
        {
            try
            {
                Models.WriteDataToFile.FileBaseDeel("C:\\" + "PeojectIni\\" + ProjiectListBox.SelectedValue.ToString() + ".ini", Models.MyFileMode.Delete);
                (this.DataContext as MainWindowModel).projiectnames.Remove(ProjiectListBox.SelectedValue.ToString());
                ProjiectListBox.SelectedIndex = 0;
                Addui(ReadIniFile("C:\\PeojectIni\\" + ProjiectListBox.SelectedValue.ToString() + ".ini"));
            }
            catch
            {
                ShowHide("已经无本地方案");

            }

        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            GetInidata("C:\\" + "PeojectIni\\" + ProjiectListBox.SelectedValue.ToString() + ".ini", GetUiData(resultlistbox));
            var a = ProjiectListBox.SelectedValue;
            if (a != null)
            {
                if (ReadIniFile("C:\\PeojectIni\\" + a.ToString() + ".ini") != null)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        (this.DataContext as MainWindowModel).SysProject = ReadIniFile("C:\\PeojectIni\\" + a.ToString() + ".ini");
                        AllAutoTest at = new AllAutoTest(ReadIniFile("C:\\PeojectIni\\" + a.ToString() + ".ini"))
                        {
                            WindowState = WindowState.Maximized
                        };
                        at.ShowDialog();
                    }));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ShowHide("方案不存在\t\n或数据错误");
                    }));
                    
                }

            }
        }
    }
}
