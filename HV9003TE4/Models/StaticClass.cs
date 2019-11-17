using LiveCharts.Defaults;
using Microsoft.Win32;
using SCEEC.Numerics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HV9003TE4.Models
{
    public static class StaticClass
    {
        public static FourTestResult AllTestResult = new FourTestResult();

        public static bool IsTcpTestting { get; set; } = false;
        public static void ShowELEYANDVOLATe(Views.EleOrVolate a)
        {
            Views.ELEYORVOLATE el = new Views.ELEYORVOLATE(a);
            el.ShowDialog();
        }
        public static byte[] getImageByte(string imagePath)
        {
            FileStream files = new FileStream(imagePath, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            return imgByte;
        }
        public static System.Drawing.Image byteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            return Bitmap.FromStream(ms, true);
        }

        public static SysAutoTestResult DeelAutoTaskData(byte[] taskdata)
        {
            if (taskdata[0] == 0xdd && taskdata[1] == 0x0a)
            {
                int TestNum = taskdata[6];
                if (TestNum > 0)
                {
                    SysAutoTestResult taskResult = new SysAutoTestResult();
                    List<float> td = new List<float>();
                    for (int i = 0; i < TestNum; i++)
                    {
                        td.Add(BitConverter.ToSingle(taskdata.Skip(7 + i * 4).Take(4).ToArray(), 0));
                    }
                    taskResult.NeedTestList = td;
                    taskResult.EleY = BitConverter.ToSingle(taskdata.Skip(7 + TestNum * 4).Take(4).ToArray(), 0);
                    taskResult.EleVolate = BitConverter.ToSingle(taskdata.Skip(7 + (TestNum + 1) * 4).Take(4).ToArray(), 0);
                    taskResult.HideTime = taskdata[taskdata.Length - 2];
                    return taskResult;
                }

            }
            return null;
        }

        public static string DeleteSpace(string text)
        {
            if (text != null)
            {
                List<char> data = new List<char>();
                char[] a = text.ToCharArray();
                foreach (var b in a)
                {
                    if (b != ' ')
                    {
                        data.Add(b);
                    }
                }
                return new string(data.ToArray());
            }
            return null;
        }

        public static bool IsOk(float Volate, float ReallyVolate)
        {
            if (Volate >= ReallyVolate * 0.8 && Volate <= ReallyVolate * 1.2)
            {
                AutoStateStatic.SState.AllNum++;
                return true;
            }
            else
            {
                AutoStateStatic.SState.NoSame++;
                AutoStateStatic.SState.AllNum++;
                return false;
            }
        }

        public static SysAutoTestResult GetDataForTcpAutoTest(byte[] data)
        {
            if (data.Length > 9)
            {
                SysAutoTestResult sys = new SysAutoTestResult();
                byte NeedTestVolate = data[6];

                for (int i = 0; i < NeedTestVolate; i++)
                {
                    sys.NeedTestList.Add(BitConverter.ToSingle(data, 9 + i * 4) * 1000);
                }
                if (data[7] == 1)
                {
                    sys.EleY = BitConverter.ToSingle(data, 9 + NeedTestVolate * 4) * 1000;
                    sys.IsEleY = true;
                }
                else
                    sys.IsEleY = false;
                if (data[8] == 1)
                {
                    sys.EleVolate = BitConverter.ToSingle(data, 9 + 4 * NeedTestVolate + data[7] * 4) * 1000;
                    sys.HideTime = data[9 + 4 * NeedTestVolate + data[7] * 4 + data[8] * 4];
                    sys.IsVolate = true;
                }
                else
                    sys.IsVolate = false;
                sys.Fre = data[data.Length - 6];

                sys.TestSpeed = BitConverter.ToSingle(data, data.Length - 5);
                return sys;
            }
            else
            {
                ShowHide("TCP传输测量数据错误");
                return null;
            }
        }

        /// <summary>
        /// 填充TCP接受的数据信息到数据源
        /// </summary>
        /// <param name="ListboxItemsources">数据源</param>
        /// <param name="SysData">TCP接受的数据</param>
        public static ObservableCollection<string> FillListBoxTip(ObservableCollection<string> ListboxItemsources, byte[] SysData)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(async pl =>
                {
                    ListboxItemsources.Add("远程数据显示\t\n若需要手动测量\t\n请设置电晕和耐压");
                    var a = StaticClass.GetDataForTcpAutoTest(SysData);
                    foreach (var b in a.NeedTestList)
                    {
                        PhysicalVariable tpd = NumericsConverter.Text2Value(b.ToString() + "V");
                        ListboxItemsources.Add("待测电压  :" + tpd);
                    }
                    PhysicalVariable eley = NumericsConverter.Text2Value(a.EleY.ToString() + "V");
                    PhysicalVariable elevolate = NumericsConverter.Text2Value(a.EleVolate.ToString() + "V");
                    ListboxItemsources.Add("待测电晕  :" + eley);
                    ListboxItemsources.Add("持续时间  :" + a.HideTime.ToString() + ":" + "  耐    压:" + elevolate);
                }, null);
            });
            return ListboxItemsources;
        }


        public static FourTestResult ReturntestTestData(FourTestResult AllTestResult, string Cn1, string Cn2, string Cn3, string Cn4,
            string Ag1, string Ag2, string Ag3, string Ag4, bool ISVOLATEFALSE)
        {
            if (ISVOLATEFALSE)
            {
                if (AllTestResult.PanelEnable)
                {
                    PanelResult p1 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value(Cn1),
                        CnTan = NumericsConverter.Text2Value(Ag1)
                    };
                    AllTestResult.Panel1Result.Add(p1);
                }
                if (AllTestResult.Pane2Enable)
                {
                    PanelResult p2 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value(Cn2),
                        CnTan = NumericsConverter.Text2Value(Ag2)
                    };
                    AllTestResult.Panel2Result.Add(p2);
                }
                if (AllTestResult.Pane3Enable)
                {
                    PanelResult p3 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value(Cn3),
                        CnTan = NumericsConverter.Text2Value(Ag3)
                    };
                    AllTestResult.Panel3Result.Add(p3);
                }
                if (AllTestResult.Pane4Enable)
                {
                    PanelResult p4 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value(Cn4),
                        CnTan = NumericsConverter.Text2Value(Ag4)
                    };
                    AllTestResult.Panel4Result.Add(p4);
                }
                return AllTestResult;
            }
            else
            {
                if (AllTestResult.PanelEnable)
                {
                    PanelResult p1 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value("0pF"),
                        CnTan = NumericsConverter.Text2Value("0")
                    };
                    AllTestResult.Panel1Result.Add(p1);
                }
                if (AllTestResult.Pane2Enable)
                {
                    PanelResult p2 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value("0pF"),
                        CnTan = NumericsConverter.Text2Value("0")
                    };
                    AllTestResult.Panel2Result.Add(p2);
                }
                if (AllTestResult.Pane3Enable)
                {
                    PanelResult p3 = new PanelResult();
                    p3.Cn = NumericsConverter.Text2Value("0pF"); p3.CnTan = NumericsConverter.Text2Value("0");
                    AllTestResult.Panel3Result.Add(p3);
                }
                if (AllTestResult.Pane4Enable)
                {
                    PanelResult p4 = new PanelResult();
                    p4.Cn = NumericsConverter.Text2Value("0pF"); p4.CnTan = NumericsConverter.Text2Value("0");
                    AllTestResult.Panel4Result.Add(p4);
                }
                return AllTestResult;
            }

        }

        public static void AddEleY(FourTestResult AllTestResult,
            PhysicalVariable eley1, PhysicalVariable eley2, PhysicalVariable eley3, PhysicalVariable eley4)
        {
            if (AllTestResult.PanelEnable)
            {
                AllTestResult.Panel1EleYAndVolate.EleY = eley1;
            }
            if (AllTestResult.Pane2Enable)
            {
                AllTestResult.Panel2EleYAndVolate.EleY = eley2;
            }
            if (AllTestResult.Pane3Enable)
            {
                AllTestResult.Panel3EleYAndVolate.EleY = eley3;
            }
            if (AllTestResult.Pane4Enable)
            {
                AllTestResult.Panel4EleYAndVolate.EleY = eley4;
            }
        }

        public static void AddEleVolate(FourTestResult AllTestResult,
            PhysicalVariable v1, PhysicalVariable v2, PhysicalVariable v3, PhysicalVariable v4,
            int h1, int h2, int h3, int h4)
        {
            if (AllTestResult.PanelEnable)
            {
                AllTestResult.Panel1EleYAndVolate.EleVolate = v1;
                AllTestResult.Panel1EleYAndVolate.HodeTime = h1;
            }
            if (AllTestResult.Pane2Enable)
            {
                AllTestResult.Panel2EleYAndVolate.EleVolate = v2;
                AllTestResult.Panel2EleYAndVolate.HodeTime = h2;
            }
            if (AllTestResult.Pane3Enable)
            {
                AllTestResult.Panel3EleYAndVolate.EleVolate = v3;
                AllTestResult.Panel3EleYAndVolate.HodeTime = h3;
            }
            if (AllTestResult.Pane4Enable)
            {
                AllTestResult.Panel4EleYAndVolate.EleVolate = v4;
                AllTestResult.Panel4EleYAndVolate.HodeTime = h4;
            }
        }
        public static byte TrueOrFalse(bool a)
        {
            if (a)
                return 0x01;
            else
                return 0x00;
        }
        public static byte[] GetTcpResult(MeasureResult result)
        {
            List<byte> rel = new List<byte>();
            rel.AddRange(new byte[] { 0xdd, 0x0a, (byte)result.TestNum });
            if (result.PanelOneEnable)
            {
                foreach (var item in result.PanelResultOne.Volatepointresult)
                {
                    PhysicalVariable cn = item.Cn;
                    PhysicalVariable cnTan = item.CnTan;
                    PhysicalVariable cnVolate = item.Volate;
                    byte[] cnf = new byte[4];
                    byte[] cntf = new byte[4];
                    byte[] cntv = new byte[4];
                    if (cn == null || cn.value == null)
                    {
                        cnf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cnf = BitConverter.GetBytes((float)cn.value);
                    if (cnTan == null || cnTan.value == null)
                    {
                        cntf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntf = BitConverter.GetBytes((float)cnTan.value);

                    if (cnVolate == null || cnVolate.value == null)
                    {
                        cntv = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntv = BitConverter.GetBytes((float)cnVolate.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                    rel.AddRange(cntv);
                }
                if (result.PanelResultOne.DYVolate == null || result.PanelResultOne.DYVolate.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultOne.DYVolate.value));
                rel.Add(TrueOrFalse(result.PanelResultOne.DyQuatity));
                rel.Add(0x01);
                rel.Add(0x01);
                if (result.PanelResultOne.KeepVolated == null || result.PanelResultOne.KeepVolated.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultOne.KeepVolated.value));
                rel.Add((byte)result.PanelResultOne.KeepTimed);
                rel.Add(0x02);
                rel.Add(0x02);
            }
            if (result.PanelTwoEnable)
            {
                foreach (var item in result.PanelResultTwo.Volatepointresult)
                {
                    PhysicalVariable cn = item.Cn;
                    PhysicalVariable cnTan = item.CnTan;
                    PhysicalVariable cnVolate = item.Volate;
                    byte[] cnf = new byte[4];
                    byte[] cntf = new byte[4];
                    byte[] cntv = new byte[4];
                    if (cn == null || cn.value == null)
                    {
                        cnf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cnf = BitConverter.GetBytes((float)cn.value);
                    if (cnTan == null || cnTan.value == null)
                    {
                        cntf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntf = BitConverter.GetBytes((float)cnTan.value);

                    if (cnVolate == null || cnVolate.value == null)
                    {
                        cntv = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntv = BitConverter.GetBytes((float)cnVolate.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                    rel.AddRange(cntv);
                }
                if (result.PanelResultTwo.DYVolate == null || result.PanelResultTwo.DYVolate.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultTwo.DYVolate.value));
                rel.Add(TrueOrFalse(result.PanelResultTwo.DyQuatity));
                rel.Add(0x01);
                rel.Add(0x01);
                if (result.PanelResultTwo.KeepVolated == null || result.PanelResultTwo.KeepVolated.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultTwo.KeepVolated.value));
                rel.Add((byte)result.PanelResultTwo.KeepTimed);
                rel.Add(0x02);
                rel.Add(0x02);
            }
            if (result.PanelThreeEnable)
            {
                foreach (var item in result.PanelResultThree.Volatepointresult)
                {
                    PhysicalVariable cn = item.Cn;
                    PhysicalVariable cnTan = item.CnTan;
                    PhysicalVariable cnVolate = item.Volate;
                    byte[] cnf = new byte[4];
                    byte[] cntf = new byte[4];
                    byte[] cntv = new byte[4];
                    if (cn == null || cn.value == null)
                    {
                        cnf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cnf = BitConverter.GetBytes((float)cn.value);
                    if (cnTan == null || cnTan.value == null)
                    {
                        cntf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntf = BitConverter.GetBytes((float)cnTan.value);

                    if (cnVolate == null || cnVolate.value == null)
                    {
                        cntv = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntv = BitConverter.GetBytes((float)cnVolate.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                    rel.AddRange(cntv);
                }
                if (result.PanelResultThree.DYVolate == null || result.PanelResultThree.DYVolate.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultThree.DYVolate.value));
                rel.Add(TrueOrFalse(result.PanelResultThree.DyQuatity));
                rel.Add(0x01);
                rel.Add(0x01);

                if (result.PanelResultThree.KeepVolated == null || result.PanelResultThree.KeepVolated.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultThree.KeepVolated.value));
                rel.Add((byte)result.PanelResultThree.KeepTimed);
                rel.Add(0x02);
                rel.Add(0x02);
            }
            if (result.PanelFourEnable)
            {
                foreach (var item in result.PanelResultFour.Volatepointresult)
                {
                    PhysicalVariable cn = item.Cn;
                    PhysicalVariable cnTan = item.CnTan;
                    PhysicalVariable cnVolate = item.Volate;
                    byte[] cnf = new byte[4];
                    byte[] cntf = new byte[4];
                    byte[] cntv = new byte[4];
                    if (cn == null || cn.value == null)
                    {
                        cnf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cnf = BitConverter.GetBytes((float)cn.value);
                    if (cnTan == null || cnTan.value == null)
                    {
                        cntf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntf = BitConverter.GetBytes((float)cnTan.value);

                    if (cnVolate == null || cnVolate.value == null)
                    {
                        cntv = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                        cntv = BitConverter.GetBytes((float)cnVolate.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                    rel.AddRange(cntv);
                }
                if (result.PanelResultFour.DYVolate == null || result.PanelResultFour.DYVolate.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultFour.DYVolate.value));
                rel.Add(TrueOrFalse(result.PanelResultFour.DyQuatity));
                rel.Add(0x01);
                rel.Add(0x01);

                if (result.PanelResultFour.KeepVolated == null || result.PanelResultFour.KeepVolated.value == null)
                {
                    rel.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                else
                    rel.AddRange(BitConverter.GetBytes((float)result.PanelResultFour.KeepVolated.value));
                rel.Add((byte)result.PanelResultFour.KeepTimed);
                rel.Add(0x02);
                rel.Add(0x02);
            }
            //rel.Add(result.Fre);
            //rel.AddRange(BitConverter.GetBytes(result.TestSpeed));
            return rel.ToArray();
        }



        public static byte[] Getbytesdata(FourTestResult fs, Int16 ImageID)
        {
            List<byte> rel = new List<byte>();
            rel.AddRange(new byte[] { 0xdd, 0x0a, (byte)fs.NeedTestNum });
            if (fs.PanelEnable)
            {
                foreach (var a in fs.Panel1Result)
                {
                    PhysicalVariable cn = a.Cn;
                    PhysicalVariable cnt = a.CnTan;
                    byte[] cnf = BitConverter.GetBytes((float)cn.value);
                    byte[] cntf = BitConverter.GetBytes((float)cnt.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                }
                if (Models.AutoStateStatic.SState.Quality)
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel1EleYAndVolate.EleY.value));
                    rel.Add(0x01);
                    rel.Add(0x01
                        );
                }
                else
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel1EleYAndVolate.EleY.value));
                    rel.Add(0x00);
                    rel.Add(0x01);
                }
                rel.AddRange(BitConverter.GetBytes((float)fs.Panel1EleYAndVolate.EleVolate.value));
                rel.Add((byte)fs.Panel1EleYAndVolate.HodeTime);
                rel.Add(0x02);
            }
            if (fs.Pane2Enable)
            {
                foreach (var a in fs.Panel2Result)
                {
                    PhysicalVariable cn = a.Cn;
                    PhysicalVariable cnt = a.CnTan;
                    byte[] cnf = BitConverter.GetBytes((float)cn.value);
                    byte[] cntf = BitConverter.GetBytes((float)cnt.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                }
                if (Models.AutoStateStatic.SState.Quality)
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel2EleYAndVolate.EleY.value));
                    rel.Add(0x01);
                    rel.Add(0x01);
                }
                else
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel2EleYAndVolate.EleY.value));
                    rel.Add(0x00);
                    rel.Add(0x01);
                }
                rel.AddRange(BitConverter.GetBytes((float)fs.Panel2EleYAndVolate.EleVolate.value));
                rel.Add((byte)fs.Panel2EleYAndVolate.HodeTime);
                rel.Add(0x02);
            }
            if (fs.Pane3Enable)
            {
                foreach (var a in fs.Panel3Result)
                {
                    PhysicalVariable cn = a.Cn;
                    PhysicalVariable cnt = a.CnTan;
                    byte[] cnf = BitConverter.GetBytes((float)cn.value);
                    byte[] cntf = BitConverter.GetBytes((float)cnt.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                }
                if (Models.AutoStateStatic.SState.Quality)
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel3EleYAndVolate.EleY.value));
                    rel.Add(0x01);
                    rel.Add(0x01);
                }
                else
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel3EleYAndVolate.EleY.value));
                    rel.Add(0x00);
                    rel.Add(0x01);
                }
                rel.AddRange(BitConverter.GetBytes((float)fs.Panel3EleYAndVolate.EleVolate.value));
                rel.Add((byte)fs.Panel3EleYAndVolate.HodeTime);
                rel.Add(0x02);
            }
            if (fs.Pane4Enable)
            {
                foreach (var a in fs.Panel4Result)
                {
                    PhysicalVariable cn = a.Cn;
                    PhysicalVariable cnt = a.CnTan;
                    byte[] cnf = BitConverter.GetBytes((float)cn.value);
                    byte[] cntf = BitConverter.GetBytes((float)cnt.value);
                    rel.AddRange(cnf);
                    rel.AddRange(cntf);
                }
                if (Models.AutoStateStatic.SState.Quality)
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel4EleYAndVolate.EleY.value));
                    rel.Add(0x01);
                    rel.Add(0x01);
                }
                else
                {
                    rel.AddRange(BitConverter.GetBytes((float)fs.Panel4EleYAndVolate.EleY.value));
                    rel.Add(0x00);
                    rel.Add(0x01);
                }
                rel.AddRange(BitConverter.GetBytes((float)fs.Panel4EleYAndVolate.EleVolate.value));
                rel.Add((byte)fs.Panel4EleYAndVolate.HodeTime);
                rel.Add(0x02);
            }
            return rel.ToArray();
        }


        public static ObservablePoint[] GetEleOrVolate(List<float> EleXvalue, List<double> EleYvalue)
        {
            if (EleXvalue.Count == EleYvalue.Count)
            {
                float[] Xva = EleXvalue.ToArray();
                double[] Yva = EleYvalue.ToArray();
                ObservablePoint[] rtd = new ObservablePoint[EleXvalue.Count];
                for (int i = 0; i < EleXvalue.Count; i++)
                {
                    rtd[i] = new ObservablePoint(Xva[i], Yva[i]);
                }
                return rtd;
            }
            return null;
        }


        public static Models.SysAutoTestResult GetSys(ObservableCollection<string> ListboxItemsources)
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

        public static void ShowHide(string Text)
        {
            MessageBox.Show(Text, "警告", MessageBoxButton.OK);
            //Views.Alarm alarm = new Views.Alarm(Text);
            //alarm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //alarm.ShowDialog();
        }

        public static void SaveImageRemote(string imagePath, string imagename, Control control)
        {
            if (!Directory.Exists(imagePath))
            {
                WriteDataToFile.DeelDirectoryInfo(imagePath, Mode.Create);
            }
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "BMP|*.bmp|PNG|*.png|JPG|*.jpg";
            SaveImg(imagePath + "\\" + imagename, control);
        }
        private static bool SaveImg(string path, Control control)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)control.ActualWidth,  //ic是控件的名字
                    (int)control.ActualHeight + 100, 1 / 48, 1 / 48, PixelFormats.Pbgra32);
                bmp.Render(control);
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

    }




    /// <summary>
    /// 
    /// </summary>
    public class SysAutoTestResult
    {
        public List<float> NeedTestList { get; set; } = new List<float>();
        public float EleY { get; set; }
        public float EleVolate { get; set; }
        public int HideTime { get; set; }

        public bool IsEleY { get; set; } = false;
        public bool IsVolate { get; set; } = false;

        public byte Fre { get; set; }
        public float TestSpeed { get; set; }

        public bool PanelOneIsOk { get; set; } = false;
        public bool PanelTwoIsOk { get; set; } = false;
        public bool PanelThreeIsOk { get; set; } = false;
        public bool PanelFourIsOk { get; set; } = false;

        public void Clear()
        {
            NeedTestList.Clear();
            EleY = 0;
            EleVolate = 0;
            HideTime = 0;
            IsEleY = false;
            IsVolate = false;
        }
    }

}
