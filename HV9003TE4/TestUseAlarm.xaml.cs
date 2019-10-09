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

namespace HV9003TE4
{
    /// <summary>
    /// TestUseAlarm.xaml 的交互逻辑
    /// </summary>
    public partial class TestUseAlarm : Window
    {
        public TestUseAlarm()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Views.Alarm a = new Views.Alarm("hello");
            a.ShowDialog();
        }
    }
}
