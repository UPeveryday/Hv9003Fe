using MahApps.Metro.Controls;
using SCEEC.Numerics;
using System;
using System.Collections.Generic;
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

namespace HV9003TE4.Views
{

    /// <summary>
    /// KeyBoard.xaml 的交互逻辑
    /// </summary>
    public partial class AddVlate : MetroWindow
    {
        public int Flag { get; set; }
        public AddVlate()
        {
            InitializeComponent();
        }
        public AddVlate(int a)
        {
            InitializeComponent();
            Flag = a;
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

        private void Mul_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int a = System.Text.RegularExpressions.Regex.Matches(ResultTextBox.Text, @"-").Count;
                if (a == 0)
                {
                    ResultTextBox.Text += "-";
                }
            }
            catch
            {
                ShowHide("输出数据格式出错");

            }
        }

        private void pF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int a = System.Text.RegularExpressions.Regex.Matches(ResultTextBox.Text, @"V").Count;
                if (a == 0)
                {
                    ResultTextBox.Text += "V";
                }
                PhysicalVariable tpd = NumericsConverter.Text2Value(ResultTextBox.Text);
                if (Flag == 1)
                    OutCnData(NumericsConverter.Value2Text((double)tpd.value, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage));
                if (Flag == 2)
                    OutVolateData(NumericsConverter.Value2Text((double)tpd.value, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage));
                if (Flag == 3)
                    OutDYData(NumericsConverter.Value2Text((double)tpd.value, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage));
                if (Flag == 4)
                    OutProjectVolate(NumericsConverter.Value2Text((double)tpd.value, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage));
                this.Close();
            }
            catch
            {
                ShowHide("输出数据格式出错");
            }
        }


        private void Nine_click(object sender, RoutedEventArgs e)
        {
            AddNum(9);

        }

        public void ShowHide(string Text)
        {
            MessageBox.Show(Text, "警告", MessageBoxButton.OK);

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
                //int a = System.Text.RegularExpressions.Regex.Matches(ResultTextBox.Text,@".").Count;
                //if (a == 1)
                //{
                ResultTextBox.Text += ".";
                // }
            }
            catch
            {
                ShowHide("结果值出错");
            }
        }

        private void Zero_click(object sender, RoutedEventArgs e)
        {
            AddNum(0);
        }


        private void uF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int a = System.Text.RegularExpressions.Regex.Matches(ResultTextBox.Text, @"V").Count;
                if (a == 0)
                {
                    ResultTextBox.Text += "kF";
                }
                PhysicalVariable tpd = NumericsConverter.Text2Value(ResultTextBox.Text);
                if (Flag == 1)
                    OutCnData(NumericsConverter.Value2Text((double)tpd.value, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage));
                if (Flag == 2)
                    OutVolateData(NumericsConverter.Value2Text((double)tpd.value, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage));
                if (Flag == 3)
                    OutDYData(NumericsConverter.Value2Text((double)tpd.value, 4, -13, "", SCEEC.Numerics.Quantities.QuantityName.Voltage));
                this.Close();
            }
            catch
            {
                ShowHide("输出数据格式出错");

            }
        }

        private void nF_Click(object sender, RoutedEventArgs e)
        {
           
            if (!ResultTextBox.Text.ToArray().Contains('V'))
            {
                ShowHide("请键入电压单位");
                ResultTextBox.Text = "";
            }
            else
                this.Close();

        }
        public event Outdy OutCnData;
        public event OutCn OutDYData;
        public event OutCn OutVolateData;
        public event OutCn OutProjectVolate;
    }
    public delegate void Outdy(string dy);
}
