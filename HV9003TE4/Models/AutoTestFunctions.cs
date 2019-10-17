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
   public static class AutoTestFunctions
    {
        public static void STAMethod(ObservableCollection<string> TestResultMeassge)
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
        private static Models.SysAutoTestResult GetSys(ObservableCollection<string> ListboxItemsources)
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
               // ShowHide("键入的耐压保持时间格式错误" + "\t\n" + "以设置为默认60S");

            }

            return sys;
        }
        //public static void StartEleY(bool IsTcpTestting,bool IsCompleteVolateTest,bool IsEnable, byte[] SysData, ObservableCollection<string> ListboxItemsources,
        //    ObservableCollection<string> TestResultMeassge)
        //{
        //    Models.SysAutoTestResult sys = new SysAutoTestResult();
        //    if (IsTcpTestting)
        //        sys = StaticClass.GetDataForTcpAutoTest(SysData);
        //    else
        //    {
        //        sys = GetSys(ListboxItemsources);
        //    }
        //    if (IsCompleteVolateTest)
        //    {
        //        //  SetVolate(sys.EleY);
        //        int p = 0;
        //        bool IsEnd = false;
        //        Models.AutoStateStatic.SState.TestText.Add("电晕  ：" + sys.EleY.ToString() + "V" + ":\t正在升压中...");
        //        STAMethod(TestResultMeassge);
        //        while (!IsEnable)
        //        {
        //            p++;
        //            Thread.Sleep(100);
        //            if (p > 5)
        //            {
        //                Models.AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
        //                Models.AutoStateStatic.SState.TestText.Add("电晕  ：" + sys.EleY.ToString() + "V" + ":\t升压超时");
        //                STAMethod(TestResultMeassge);
        //                ThreadPool.QueueUserWorkItem(delegate
        //                {
        //                    SynchronizationContext.SetSynchronizationContext(new
        //                    System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
        //                    SynchronizationContext.Current.Post(async pl =>
        //                    {
        //                        if (sys.IsVolate)
        //                        {
        //                            StaticClass.ShowELEYANDVOLATe(Views.EleOrVolate.Volate);
        //                            if (AutoStateStatic.SState.IsStartVolate == true)
        //                                await Task.Factory.StartNew(StartVolate);
        //                            else
        //                                AutoStateStatic.SState.IsStartVolate = false;
        //                        }
        //                        else
        //                        {
        //                            IsTcpTestting = false;
        //                        }
        //                    }, null);
        //                });
        //                IsEnd = true;
        //                break;
        //            }
        //        }
        //        if (!IsEnd)
        //        {
        //            AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
        //            AutoStateStatic.SState.TestText.Add("电晕  ：" + sys.EleY.ToString() + "V" + ":\t升压完成");
        //            STAMethod(TestResultMeassge);
        //            ThreadPool.QueueUserWorkItem(delegate
        //            {
        //                SynchronizationContext.SetSynchronizationContext(new
        //                System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher));
        //                SynchronizationContext.Current.Post(async pl =>
        //                {
        //                    Views.Qualified q = new Views.Qualified
        //                    {
        //                        WindowStartupLocation = WindowStartupLocation.CenterScreen
        //                    };
        //                    q.ShowDialog();
        //                    StaticClass.ShowELEYANDVOLATe(Views.EleOrVolate.Volate);
        //                    if (AutoStateStatic.SState.IsStartVolate == true)
        //                        await Task.Factory.StartNew(StartVolate);
        //                    else
        //                        AutoStateStatic.SState.IsStartVolate = false;
        //                }, null);
        //            });
        //        }
        //    }

        //}

        public static void StartVolate(bool IsTcpTestting,bool IsCompleteEleTest,bool IsEnable, PhysicalVariable HVVoltage, byte[] SysData, ObservableCollection<string> ListboxItemsources,
            ObservableCollection<string> TestResultMeassge)
        {
            Models.SysAutoTestResult sys = new SysAutoTestResult();
            if (IsTcpTestting)
                sys = StaticClass.GetDataForTcpAutoTest(SysData);
            else
            {
                sys = GetSys(ListboxItemsources);
            }
            if (IsCompleteEleTest)
            {
                //  SetVolate(sys.EleVolate);
                int p = 0;
                bool IsEnd = false;
                Models.AutoStateStatic.SState.TestText.Add("耐压  ：" + sys.EleVolate.ToString() + "V" + ":\t正在升压中...");
                STAMethod(TestResultMeassge);
                while (!IsEnable)
                {
                    p++;
                    Thread.Sleep(100);
                    float maxvalue = 0;
                    float actvalue = 0;
                    if (p > 5)
                    {
                        if (Math.Abs((float)HVVoltage.value - sys.EleVolate) < maxvalue)
                            actvalue = (float)HVVoltage.value;
                        AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                        AutoStateStatic.SState.TestText.Add("耐压  ：" + actvalue.ToString() + "V" + ":\t未升到耐压值");
                        STAMethod(TestResultMeassge);
                        IsEnd = true;
                        IsTcpTestting = false;
                        break;
                    }
                }
                if (!IsEnd)
                {
                    AutoStateStatic.SState.TestText.RemoveAt(AutoStateStatic.SState.TestText.Count - 1);
                    AutoStateStatic.SState.TestText.Add("耐压  ：" + sys.EleVolate.ToString() + "V" + ":\t升压完成");
                    STAMethod(TestResultMeassge);
                    int c = 0;
                    float MaxVolate = 0;
                    #region 
                    while (true)
                    {
                        Models.AutoStateStatic.SState.TestText.Add("当前电压： " + HVVoltage + "\t\n正在持续耐压中...");
                        STAMethod(TestResultMeassge);
                        if (c < sys.HideTime * 2)
                        {
                            if (StaticClass.IsOk((float)HVVoltage.value, sys.EleVolate))
                            {
                                float tempdata = Math.Abs(sys.EleVolate - (float)HVVoltage.value);
                                if (tempdata > MaxVolate)
                                {
                                    MaxVolate = tempdata;
                                    AutoStateStatic.SState.MaxEqualVolate = sys.EleVolate;
                                }
                                Thread.Sleep(500);
                                c++;
                            }
                            else
                            {
                                Thread.Sleep(500);
                                c++;
                                continue;
                            }
                        }
                        else
                        {
                            if (((float)AutoStateStatic.SState.NoSame / (float)AutoStateStatic.SState.AllNum) >= 0.80f)
                            {
                                Models.AutoStateStatic.SState.TestText.Add("耐压实验已完成  耐压结果： 不合格");
                                STAMethod(TestResultMeassge);
                                AutoStateStatic.SState.NaiVolate = false;//耐压是否合格
                                AutoStateStatic.SState.CompeleteVolate = true;//是否完成耐压
                                IsTcpTestting = false;
                                break;
                            }
                            else
                            {
                                Models.AutoStateStatic.SState.TestText.Add("耐压实验已完成  耐压结果： 合格");
                                STAMethod(TestResultMeassge);
                                AutoStateStatic.SState.NaiVolate = true;//耐压是否合格
                                AutoStateStatic.SState.CompeleteVolate = true;//是否完成耐压
                                IsTcpTestting = false;
                                break;
                            }
                        }
                    }
                    #endregion
                }
            }
        }


    }
}
