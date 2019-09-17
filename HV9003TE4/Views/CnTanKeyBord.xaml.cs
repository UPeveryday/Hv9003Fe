using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using SCEEC.Numerics;


namespace HV9003TE4.Views
{
    public delegate void OutCntan(string cntan);
    /// <summary>
    /// CnTanKeyBord.xaml 的交互逻辑
    /// </summary>
    public partial class CnTanKeyBord : MetroWindow
    {
        public CnTanKeyBord()
        {
            InitializeComponent();
        }


        private void Delete_click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBox.Text != "")
            {
                ResultTextBox.Text = ResultTextBox.Text.Remove(ResultTextBox.Text.Length - 1);
            }
        }


        private void E_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = "";
        }


        private void Nine_click(object sender, RoutedEventArgs e)
        {
            AddNum(9);

        }

        private void Eight_click(object sender, RoutedEventArgs e)
        {
            AddNum(8);

        }

        private void Seven_click(object sender, RoutedEventArgs e)
        {
            AddNum(7);

        }

        private void Six_click(object sender, RoutedEventArgs e)
        {
            AddNum(6);

        }

        private void One_click(object sender, RoutedEventArgs e)
        {
            AddNum(1);

        }

        private void Two_click(object sender, RoutedEventArgs e)
        {
            AddNum(2);

        }

        private void Three_click(object sender, RoutedEventArgs e)
        {
            AddNum(3);

        }

        private void Four_click(object sender, RoutedEventArgs e)
        {
            AddNum(4);

        }

        private void Five_click(object sender, RoutedEventArgs e)
        {
            AddNum(5);
        }
        private void AddNum(int Num)
        {
            ResultTextBox.Text += Num.ToString();
        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResultTextBox.Text += ".";
            }
            catch
            {
                throw new Exception("结果值出错");
            }
        }

        private void Zero_click(object sender, RoutedEventArgs e)
        {
            AddNum(0);
        }

        private void Per_click(object sender, RoutedEventArgs e)
        {
            try
            {
                int a = System.Text.RegularExpressions.Regex.Matches(ResultTextBox.Text, @"%").Count;
                if (a == 0)
                {
                    ResultTextBox.Text += "%";
                }

            }
            catch
            {
                throw new Exception("结果值出错");
            }
        }
        public event OutCntan OutCnTanData;
        private void Confire_click(object sender, RoutedEventArgs e)
        {
            PhysicalVariable tpd = NumericsConverter.Text2Value(ResultTextBox.Text);
            //  NumericsConverter.Value2Text(tpd,4,percentage:true);
            OutCnTanData(NumericsConverter.Value2Text(tpd, percentage: true));
            this.Close();
        }

        private static DispatcherTimer myClickWaitTimer =
     new DispatcherTimer(
         new TimeSpan(0, 0, 0, 1),
         DispatcherPriority.Background,
         mouseWaitTimer_Tick,
         Dispatcher.CurrentDispatcher);
        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Stop the timer from ticking.
            myClickWaitTimer.Stop();
            Trace.WriteLine("Double Click");
            e.Handled = true;
            ResultTextBox.Text = ResultTextBox.Text.Remove(ResultTextBox.Text.Length - 1);
            ResultTextBox.Text += "+";

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myClickWaitTimer.Start();
            ResultTextBox.Text += "-";

        }
        private static void mouseWaitTimer_Tick(object sender, EventArgs e)
        {
            myClickWaitTimer.Stop();
            // Handle Single Click Actions
            Trace.WriteLine("Single Click");
        }




        private void Addbutton_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (e.ClickCount)
            {
                case 1://单击
                    {
                        ResultTextBox.Text += "+";
                        break;
                    }
                case 2://双击
                    {
                        ResultTextBox.Text += "-";
                        break;
                    }
            }
        }
    }
}
