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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HV9003TE4.Models
{
    public static class StaticClass
    {

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
        public static Image byteArrayToImage(byte[] Bytes)
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
            SysAutoTestResult sys = new SysAutoTestResult();
            byte NeedTestVolate = data[6];
            if (data[0] == 0xdd && data[1] == 0x0a)
            {
                for (int i = 0; i < NeedTestVolate; i++)
                {
                    sys.NeedTestList.Add(BitConverter.ToSingle(data, 9 + i * 4));
                }
                if (data[7] == 1)
                {
                    sys.EleY = BitConverter.ToSingle(data, 9 + NeedTestVolate * 8);
                    sys.IsEleY = true;
                }
                else
                    sys.IsEleY = false;
                if (data[8] == 1)
                {
                    sys.EleVolate = BitConverter.ToSingle(data, 9 + 8 * NeedTestVolate + data[7] * 4);
                    sys.HideTime = data[9 + 8 * NeedTestVolate + data[7] * 4 + data[8] * 4];
                    sys.IsVolate = true;
                }
                else
                    sys.IsVolate = false;

                return sys;
            }
            return null;
        }

        /// <summary>
        /// 填充TCP接受的数据信息到数据源
        /// </summary>
        /// <param name="ListboxItemsources">数据源</param>
        /// <param name="SysData">TCP接受的数据</param>
        public static void FillListBoxTip(ObservableCollection<string> ListboxItemsources, byte[] SysData)
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
        }


        public static FourTestResult ReturntestTestData(FourTestResult AllTestResult, string Cn1, string Cn2, string Cn3, string Cn4,
            string Ag1, string Ag2, string Ag3, string Ag4, bool pan1, bool pan2, bool pan3, bool pan4, bool ISVOLATEFALSE)
        {
            if (ISVOLATEFALSE)
            {
                if (!pan1)
                {
                    PanelResult p1 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value(Cn1),
                        CnTan = NumericsConverter.Text2Value(Ag1)
                    };
                    AllTestResult.Panel1Result.Add(p1);
                }
                if (!pan2)
                {
                    PanelResult p2 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value(Cn2),
                        CnTan = NumericsConverter.Text2Value(Ag2)
                    };
                    AllTestResult.Panel2Result.Add(p2);
                }
                if (!pan3)
                {
                    PanelResult p3 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value(Cn3),
                        CnTan = NumericsConverter.Text2Value(Ag3)
                    };
                    AllTestResult.Panel3Result.Add(p3);
                }
                if (!pan4)
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
                if (!pan1)
                {
                    PanelResult p1 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value("0pF"),
                        CnTan = NumericsConverter.Text2Value("0")
                    };
                    AllTestResult.Panel1Result.Add(p1);
                }
                if (!pan2)
                {
                    PanelResult p2 = new PanelResult
                    {
                        Cn = NumericsConverter.Text2Value("0pF"),
                        CnTan = NumericsConverter.Text2Value("0")
                    };
                    AllTestResult.Panel2Result.Add(p2);
                }
                if (!pan3)
                {
                    PanelResult p3 = new PanelResult();
                    p3.Cn = NumericsConverter.Text2Value("0pF"); p3.CnTan = NumericsConverter.Text2Value("0");
                    AllTestResult.Panel3Result.Add(p3);
                }
                if (!pan4)
                {
                    PanelResult p4 = new PanelResult();
                    p4.Cn = NumericsConverter.Text2Value("0pF"); p4.CnTan = NumericsConverter.Text2Value("0");
                    AllTestResult.Panel4Result.Add(p4);
                }
                return AllTestResult;
            }

        }

        public static void AddEleY(FourTestResult AllTestResult, bool p1, bool p2, bool p3, bool p4,
            PhysicalVariable eley1, PhysicalVariable eley2, PhysicalVariable eley3, PhysicalVariable eley4)
        {
            if (p1)
            {
                AllTestResult.Panel1EleYAndVolate.EleY = eley1;
            }
            if (p2)
            {
                AllTestResult.Panel2EleYAndVolate.EleY = eley2;
            }
            if (p3)
            {
                AllTestResult.Panel3EleYAndVolate.EleY = eley3;
            }
            if (p4)
            {
                AllTestResult.Panel4EleYAndVolate.EleY = eley4;
            }
        }

        public static void AddEleVolate(FourTestResult AllTestResult, bool p1, bool p2, bool p3, bool p4,
            PhysicalVariable v1, PhysicalVariable v2, PhysicalVariable v3, PhysicalVariable v4,
            int h1, int h2, int h3, int h4)
        {
            if (p1)
            {
                AllTestResult.Panel1EleYAndVolate.EleVolate = v1;
                AllTestResult.Panel1EleYAndVolate.HodeTime = h1;
            }
            if (p2)
            {
                AllTestResult.Panel2EleYAndVolate.EleVolate = v2;
                AllTestResult.Panel2EleYAndVolate.HodeTime = h2;
            }
            if (p3)
            {
                AllTestResult.Panel3EleYAndVolate.EleVolate = v3;
                AllTestResult.Panel3EleYAndVolate.HodeTime = h3;
            }
            if (p4)
            {
                AllTestResult.Panel4EleYAndVolate.EleVolate = v4;
                AllTestResult.Panel4EleYAndVolate.HodeTime = h4;
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
