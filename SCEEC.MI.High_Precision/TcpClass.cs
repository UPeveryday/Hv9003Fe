using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace SCEEC.MI.High_Precision
{
    public class Informations
    {
        public struct MyInfo
        {
            public Socket socket;//传入的socket对象
            public IntPtr Handle;//传入的句柄
            public string Text;// 传入的Text值
            public byte[] Data;//接受缓冲区数据
        }
    }
    public struct Messages
    {
        //自定义消息区域
        public const int WM_USER = 0x100;
        public const int Wm_Change_BtnTxt = WM_USER + 1;
        public const int Wm_Receive_Info = WM_USER + 2;
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            IntPtr hWnd,//消息发生串口句柄
            int msg,//消息号
            int wParam,//参数1
            ref Informations.MyInfo IParam//参数2
            );
    }

    public class Work
    {
        public Informations.MyInfo info;
        public static ManualResetEvent alldone = new ManualResetEvent(false);
        public void receivedata()
        {
            try
            {
                while (true)
                {
                    alldone.Reset();
                    if (info.socket != null)
                    {
                        info.socket.BeginReceive(info.Data, 0, info.Data.Length, SocketFlags.None, new AsyncCallback(CallbackReceive), info);
                    }
                    alldone.WaitOne();
                }
            }
            catch (SocketException)
            {
            }
        }

        private static void CallbackReceive(IAsyncResult ar)
        {
            //throw new NotImplementedException();
            alldone.Set();
            Informations.MyInfo info_ = (Informations.MyInfo)ar.AsyncState;
            int length = 0;
            try
            {
                length = info_.socket.EndReceive(ar);
            }
            catch (Exception)
            {

                throw;
            }
            if (length > 0)
            {
                info_.Text = "<<<接受信息成功>>>" + Encoding.UTF8.GetString(info_.Data);
                Array.Clear(info_.Data, 0, info_.Data.Length);
                Messages.SendMessage(info_.Handle, Messages.Wm_Receive_Info, 0, ref info_);
            }
        }
    }


















    class TcpClass
    {
        public string ip;
        public int port;
        public TcpClass(string ip,int port)
        {
            this.ip = ip;
            this.port = port;
        }

        //TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ip), port));//ip为服务器IP地址，port为监听的端口

        //Listener.Start();//开启监听

    }
}
